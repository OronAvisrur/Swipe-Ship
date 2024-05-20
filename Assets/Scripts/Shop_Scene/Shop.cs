using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    [Header("Prefabs And Sprites Components")]
    [SerializeField] private Item _itemPrefab;
    [SerializeField] private Sprite[] _shipSprites; 

    [Header("UI Components")]
    [SerializeField] private TMP_Text _coinAmountText;
    [SerializeField] private GameObject _shopItemsContainer;

    [Header("Audio Component")]
    [SerializeField] private AudioClip _purchaseAudioClip;

    private enum Ships { White, Black, Red, Green, Blue, Yellow, Ghost};
    private List<int> ownedShips;

    private Vector3 posToSpawn;
    
    private AudioSource _audioSource;
    private int coinsAmount;
    private int _inUseShip = 0;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource is null) Debug.LogError("Audio Source Not Found!");

        coinsAmount = PlayerPrefs.GetInt("Coins", 0);
        _coinAmountText.text = coinsAmount.ToString();
        _inUseShip = PlayerPrefs.GetInt("InUseShip", 0);
        ownedShips = SavingSystem.LoadShips();

        if(ownedShips is null)
        {
            ownedShips = new List<int>();
            ownedShips.Add(0);
            SavingSystem.SaveShipsList(ownedShips);
        }

        _audioSource.clip = _purchaseAudioClip;

        LoadShopItems();
    }

    public void LoadShopItems(int amount = 7)
    {
        posToSpawn = new Vector3(0, 0, 0);

        if(ownedShips != null)
        {
            foreach (Ships ship in Enum.GetValues(typeof(Ships)))
            {
                Item newItem = Instantiate(_itemPrefab, posToSpawn, Quaternion.identity);
                newItem.transform.SetParent(_shopItemsContainer.transform, false); 

                newItem.ID = (int)ship;
                newItem.ShipSprite = _shipSprites[(int)ship];

                switch (ship)
                {
                    case Ships.White:
                        newItem.Name = "White Ship";
                        newItem.Price = 1000;
                        break;
                    case Ships.Black:
                        newItem.Name = "Black Ship";
                        newItem.Price = 1500;
                        break;
                    case Ships.Red:
                        newItem.Name = "Red Ship";
                        newItem.Price = 3000;
                        break;
                    case Ships.Green:
                        newItem.Name = "Green Ship";
                        newItem.Price = 6000;
                        break;
                    case Ships.Blue:
                        newItem.Name = "Blue Ship";
                        newItem.Price = 7000;
                        break;
                    case Ships.Yellow:
                        newItem.Name = "Yellow Ship";
                        newItem.Price = 8500;
                        break;
                    case Ships.Ghost:
                        newItem.Name = "Ghost Ship";
                        newItem.Price = 9500;
                        break;
                    default:
                        newItem.Name = "Unkown";
                        newItem.Price = 0;
                        break;
                }
            }

            LoadPricesAndInventory();
        }
        else Debug.LogError("Can't Load Shop!");

    }

    private void LoadPricesAndInventory()
    {
        for(int i = 0; i < ownedShips.Count; i++)
        {
            if(_inUseShip == ownedShips[i]) _shopItemsContainer.transform.GetChild(ownedShips[i]).GetComponent<Item>().Price = 1;
            else  _shopItemsContainer.transform.GetChild(ownedShips[i]).GetComponent<Item>().Price = 0;
        }
    }
    
    public void BuyItem(int itemID, int price)
    {
        if(coinsAmount > price)
        {
            if(PlayerPrefs.GetInt("SoundState", 1) == 1 ? true : false) _audioSource.Play();

            coinsAmount -= price;
            _coinAmountText.text = coinsAmount.ToString();
            PlayerPrefs.SetInt("Coins", coinsAmount);

            ownedShips.Add(itemID);
            SavingSystem.SaveShipsList(ownedShips);
        }
        else StartCoroutine(NotEnoughCoins());

        LoadPricesAndInventory();
        
    }

    public void SwitchShip(int shipID)
    {
        _shopItemsContainer.transform.GetChild(_inUseShip).GetComponent<Item>().Price = 0;
        _shopItemsContainer.transform.GetChild(shipID).GetComponent<Item>().Price = 1;

        PlayerPrefs.SetInt("InUseShip", shipID);
        _inUseShip = shipID;

        LoadPricesAndInventory();
    }

    private IEnumerator NotEnoughCoins()
    {
        _coinAmountText.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _coinAmountText.color = new Color32(253, 151, 68, 255);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(1);
    }
}
