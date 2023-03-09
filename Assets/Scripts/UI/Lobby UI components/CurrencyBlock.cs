using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyBlock : MonoBehaviour
{
    [Header("Predefined bets")]
    [SerializeField] private string _predefinedBetTextPrefix;
    [SerializeField] private string _predefinedBetTextPostfix;
    [SerializeField] private GameObject _predefinedBetPrefab;
    [SerializeField] private Transform _predefinedBetsParent;
    [Header("Custom bet")]
    [SerializeField] private Slider _customBetSlider;
    [SerializeField] private TMP_Text _customBetText;
    private List<Button> _predefindeBetButtons = new List<Button>();
}
