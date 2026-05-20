using MapEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Gameplay;
using System;
using System.Collections.Generic;
using UnityEditor.Rendering;

public class MapEditorMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField _col, _row;
    [SerializeField] TMP_Dropdown _colorDropdown;
    [SerializeField] Button _submitButton;
    [SerializeField] Button _nextButton, _prevButton;

    [SerializeField] BoardEditor _boardEditor;
    [SerializeField] Hole _selectedHole;


    private void Start()
    {
        _submitButton.onClick.AddListener(OnSubmit);
        _nextButton.onClick.AddListener(OnNext);
        _prevButton.onClick.AddListener(OnPrev);
        InitColorDropdown();
    }

    private void InitColorDropdown()
    {
        _colorDropdown.options.Clear();
        foreach (SheepColor color in Enum.GetValues(typeof(SheepColor)))
        {
            if (color == SheepColor.None) continue;
            _colorDropdown.options.Add(new TMP_Dropdown.OptionData(color.ToString()));
        }
        _colorDropdown.onValueChanged.AddListener(OnColorChanged);

        // Set initial color
        _boardEditor.CurrentColor = (SheepColor)Enum.Parse(typeof(SheepColor), _colorDropdown.options[_colorDropdown.value].text);
    }

    private void OnColorChanged(int index)
    {
        string colorName = _colorDropdown.options[index].text;
        _boardEditor.CurrentColor = (SheepColor)Enum.Parse(typeof(SheepColor), colorName);

        if (_selectedHole != null)
        {
            SheepColor color = (SheepColor)Enum.Parse(typeof(SheepColor), colorName);
            _selectedHole.SetColor(color);
        }
    }

    private void OnSubmit()
    {
        int col = int.Parse(_col.text);
        int row = int.Parse(_row.text);

        _boardEditor.UpdateBoardSize(col, row);
    }

    public void SelectHole(Hole h)
    {
        _selectedHole = h;
        _colorDropdown.value = (int)h.Color;
    }

    private void OnNext()
    {
        if (_selectedHole == null)
            return;

        _boardEditor.NextHole(_selectedHole);
    }

    private void OnPrev()
    {
        if (_selectedHole == null)
            return;

        _boardEditor.PrevHole(_selectedHole);
    }
}
