using TMPro;
using UnityEngine;

public class GetValueFromDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    public void GetDropdownValue()
    {
        int index = dropdown.value;
        string selectedValue = dropdown.options[index].text;

    }
}
