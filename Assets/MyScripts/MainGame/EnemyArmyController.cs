using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyArmyController : MonoBehaviour
{
    [SerializeField] public List<DestructionTargetScript> targets;
    [SerializeField] public List<DestructionTargetScript> selectedTargets;
    [SerializeField] public List<DestructionTargetScript> destoryedTargets;
    [SerializeField] public GameObject DrarfTemplate;
    public List<EnemyColectorAI> agents;
    public int EnemyCount;


    private void Awake()
    {
        DrarfTemplate.SetActive(false);
        targets = FindObjectsOfType(typeof(DestructionTargetScript))
                  .Cast<DestructionTargetScript>()
                  .ToList();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        for (int i = 0; i < 25; i++)
        {
            var agent = Instantiate(DrarfTemplate);
            agent.transform.position = DrarfTemplate.transform.position;
            agent.SetActive(true);
            agent.transform.SetParent(transform, true);
            agent.name = $"Enemy {i}";
            var ai = agent.GetComponent<EnemyColectorAI>();
            ai.OnTargetDestroyed = TargetDestoryed;
            ai.ObtainTarget = GetNextTarget;
            ai.BeginAttack();

            yield return new WaitForSeconds(Random.Range(1f, 4f));
        }
    }

    public DestructionTargetScript GetNextTarget()
    {
        var target = targets.OrderBy(x => Random.Range(0f, 1f) * x.Dificulty).FirstOrDefault();
        targets.Remove(target);
        selectedTargets.Add(target);
        return target;
    }

    public void TargetDestoryed(DestructionTargetScript target)
    {
        selectedTargets.Remove(target);
        destoryedTargets.Add(target);
    }
}
