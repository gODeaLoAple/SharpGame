using System.Collections;
using UnityEngine;
using Random = System.Random;

namespace C__scripts.Enemies
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] public int cost;
        private Animator animator;
        private new GameObject gameObject;
        private new Transform transform;
        private Transform spawnPosition;
        private GameObject player;
        private Canvas canvasForFight;
        private Entity entity;
        private Fight fight;
        private SpriteRenderer spriteRenderer;
        
        private float radius;
        private float speed;

        private float position;
        private bool isMoveRight = true;
        private static readonly int Go = Animator.StringToHash("go");
        private static readonly int AttackAnimation = Animator.StringToHash("attack");
        private static readonly int Idle = Animator.StringToHash("idle");
        private static readonly int CriticalDamage = Animator.StringToHash("criticalDamage");

        public EnemyState State { get; private set; }

        public void Init(GameObject gameObject, float radius, float speed, Transform spawnPosition, GameObject player, Canvas canvas, Fight fight)
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            this.gameObject = gameObject;
            position = spawnPosition.position.x;
            animator = gameObject.GetComponent<Animator>();
            this.radius = radius;
            this.speed = speed;
            this.spawnPosition = spawnPosition;
            this.player = player;
            transform = gameObject.transform;
            transform.position = spawnPosition.transform.position;
            State = EnemyState.Born;
            canvasForFight = canvas;
            this.fight = fight;
            entity = gameObject.GetComponent<Entity>();
        }
        
        public void FixedUpdate()
        {
            if (gameObject is null) return;
            switch (State)
            {
                case EnemyState.Born:
                    State = EnemyState.Move;
                    return;
                case EnemyState.Move:
                    Move();
                    return;
            }
        }

        private void Move()
        {
            position = transform.position.x;
            var vector = Vector2.right * (speed * Time.deltaTime);
            
            if (position + vector.x < spawnPosition.position.x + radius && isMoveRight)
            {
                transform.Translate(vector);
            }
            else if (position - vector.x > spawnPosition.position.x - radius)
            {
                isMoveRight = false;
                transform.eulerAngles = new Vector3(0, -180, 0);
                transform.Translate(vector);
            }
            else
            {
                isMoveRight = true;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            
            animator.SetTrigger(Go);
        }

        public IEnumerator Attack()
        {
            var originalColor = spriteRenderer.color;
            if (new Random().Next(0, 101) < 20)
                entity.UseSkills();
            var geolocationNow = transform.position.x;
            var geolocationPlayer = player.transform.position.x;
            
            yield return new WaitForSeconds(0.3f);
            
            animator.SetTrigger(Go);
            transform.eulerAngles = new Vector3(0, -180, 0);
            while (transform.position.x > geolocationPlayer + 2)
            {
                transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
                yield return null;
            }
            var component = GetComponent<Entity>(); 
            if (component.HasHealthDecreased())
            {
                spriteRenderer.color = Color.red; 
                yield return new WaitForSeconds(0.5f); 
            }


            animator.SetTrigger(!fight.critDamage ? AttackAnimation : CriticalDamage);
            yield return new WaitForSeconds(0.8f);
            
            transform.eulerAngles = new Vector3(0, 0, 0);
            while (transform.position.x < geolocationNow)
            {
                transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
                yield return null;
            }
            transform.eulerAngles = new Vector3(0, -180, 0);
            yield return new WaitForSeconds(1f);
            spriteRenderer.color = originalColor;
            yield return null;
            animator.SetTrigger(Idle);
        }

        public IEnumerator Die()
        {
            animator.SetTrigger("die");
            yield return new WaitForSeconds(0.35f);
            Destroy(gameObject);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && State is EnemyState.Move)
            {
                transform.position += new Vector3(1f, 0);
                transform.eulerAngles = new Vector3(0, -180, 0);
                State = EnemyState.Fight;
                player.GetComponent<Player>().speed = 0;
                player.GetComponent<Player>().fight = true;
                fight = Instantiate(fight);
                fight.Init(player, canvasForFight, gameObject);
                entity.ShowCanvas();
                animator.SetTrigger(Idle);
            }
        }
    }
}
