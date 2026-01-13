using UnityEngine;

public class ConsumableManager : MonoBehaviour
{
    public Transform itemHolder;

    private GameObject currentItem;


    public void EquipConsumable(ConsumableItemData data)
    {
        if (currentItem != null)
            Destroy(currentItem);

        currentItem = Instantiate(data.equippedPrefab, itemHolder);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;
        currentItem.transform.localScale = Vector3.one;


        PotionInHand potion = currentItem.GetComponent<PotionInHand>();
        if (potion != null)
        {
            potion.healAmount = data.healAmount;
        }
    }


    void Update()
    {
        if (currentItem != null && Input.GetMouseButtonDown(0))
        {
            PotionInHand potion = currentItem.GetComponent<PotionInHand>();
            if (potion != null)
            {
                potion.Use(gameObject);
                currentItem = null;
            }
        }
    }

    public void Unequip()
    {
        if (currentItem != null)
        {
            Destroy(currentItem);
            currentItem = null;
        }
    }


}
