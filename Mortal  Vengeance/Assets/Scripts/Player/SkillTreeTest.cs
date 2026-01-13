using UnityEngine;

public class SkillTreeTest : MonoBehaviour
{
    private void Update()
    {
        // Press T to test unlocking a skill
        if (Input.GetKeyDown(KeyCode.T))
        {
            SkillTreeManager.instance.TryUnlockSkill("hlt_maxhealth_1");
            Debug.Log("Attempted to unlock hlt_maxhealth_1");
        }
    }
}