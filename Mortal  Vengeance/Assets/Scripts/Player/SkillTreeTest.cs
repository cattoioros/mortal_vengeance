using UnityEngine;

public class SkillTreeTest : MonoBehaviour
{
    private void Update()
    {
        // Press T to test unlocking a skill
        if (Input.GetKeyDown(KeyCode.T))
        {
            SkillTreeManager.instance.TryUnlockSkill("str_health_1");
            Debug.Log("Attempted to unlock str_health_1");
        }
    }
}