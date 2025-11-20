using UnityEngine;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject cardSelectionUI;
    [SerializeField] private List<CardSO> deck;

    [Header("Card Slots")]
    [SerializeField] private Transform cardSlot1;
    [SerializeField] private Transform cardSlot2;
    [SerializeField] private Transform cardSlot3;

    [SerializeField] private GameObject cardUIPrefab;

    private readonly List<CardSO> alreadySelectedCards = new List<CardSO>();
    public static CardManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    
    public void RandomizeNewCards()
    {
        // เคลียร์ของเก่า
        foreach (Transform child in cardSlot1) Destroy(child.gameObject);
        foreach (Transform child in cardSlot2) Destroy(child.gameObject);
        foreach (Transform child in cardSlot3) Destroy(child.gameObject);

        // สุ่มการ์ด 3 ใบ
        var randomizedCards = GetRandomCards(3);
        if (randomizedCards.Count < 3)
        {
            Debug.LogWarning("Not enough available cards to show.");
            return;
        }

        // สร้าง UI สำหรับแต่ละใบ
        CreateCardUI(randomizedCards[0], cardSlot1);
        CreateCardUI(randomizedCards[1], cardSlot2);
        CreateCardUI(randomizedCards[2], cardSlot3);
    }

    private void CreateCardUI(CardSO cardData, Transform slot)
    {
        var cardUI = Instantiate(cardUIPrefab, slot);
        var card = cardUI.GetComponent<Card>();
        card.Setup(cardData);
    }

    
    private List<CardSO> GetRandomCards(int count)
    {
        // กรองเด็ค: ถ้าเป็น unique และเคยเลือกไปแล้ว ให้ตัดออก
        var available = new List<CardSO>(deck);
        available.RemoveAll(c => c == null);
        available.RemoveAll(c => c.isUnique && alreadySelectedCards.Contains(c));

        // Fisher–Yates shuffle แบบง่าย
        for (int i = 0; i < available.Count; i++)
        {
            int j = Random.Range(i, available.Count);
            (available[i], available[j]) = (available[j], available[i]);
        }

        // คืนจำนวนที่มี (อาจ < count ถ้ามีน้อย)
        if (available.Count > count) return available.GetRange(0, count);
        return available;
    }

    public void SelectCard(CardSO selectedCard)
    {
        if (selectedCard == null) return;

        if (selectedCard.isUnique && !alreadySelectedCards.Contains(selectedCard))
            alreadySelectedCards.Add(selectedCard);

        selectedCard.ApplyCardEffect(); // ✅ ใช้การ์ดจริง
        Debug.Log("Card selected: " + selectedCard.cardName);

        OnCardPicked(); // ✅ ปิด UI และเริ่ม wave ต่อไป
    }
    
    public void OnCardPicked()
    {
        HideCardSelection();
        // ไม่ต้องเปลี่ยน state เองที่นี่ ให้ GameManager เป็นคนเริ่มเวฟถัดไป
        StartCoroutine(GameManager.Instance.StartNextWaveAfterCard());
    }
    
    
    
    public void ShowCardSelection()
    {
        Debug.Log("Showing Card UI");
        cardSelectionUI.SetActive(true);
    }
    public void HideCardSelection()  => cardSelectionUI.SetActive(false);
}
