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
        var available = new List<CardSO>();

        // wave ปัจจุบัน (ถ้าเผื่อไม่มี GameManager ให้ใช้ 1)
        int currentWave = 1;
        if (GameManager.Instance != null)
            currentWave = GameManager.Instance.CurrentWave;

        foreach (var card in deck)
        {
            if (card == null)
                continue;

            // 1) การ์ด unique ที่เคยเลือกไปแล้วใน run นี้ → ไม่เอาแล้ว
            if (card.isUnique && alreadySelectedCards.Contains(card))
                continue;

            // 2) unlockLevel > 0 = ต้องรอ wave ถึงก่อน
            //    ถ้า unlockLevel = 0 แปลว่าไม่ล็อก ใช้ได้ทุก wave
            bool lockedByWave = (card.unlockLevel > 0 && currentWave < card.unlockLevel);
            if (lockedByWave)
                continue;

            available.Add(card);
        }

        // 3) ลบการ์ดที่ซ้ำในเด็คออก (ให้เหลือใบละ 1)
        var unique = new List<CardSO>();
        var seen = new HashSet<CardSO>();
        foreach (var c in available)
        {
            if (seen.Add(c))   // ถ้าเพิ่งเจอครั้งแรก
                unique.Add(c);
        }
        available = unique;

        // 4) Fisher–Yates shuffle
        for (int i = 0; i < available.Count; i++)
        {
            int j = Random.Range(i, available.Count);
            (available[i], available[j]) = (available[j], available[i]);
        }

        // 5) คืนจำนวนที่มี (อาจ < count ถ้าการ์ดที่เข้าเงื่อนไขมีน้อย)
        if (available.Count > count)
            return available.GetRange(0, count);

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
