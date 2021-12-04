using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName ="Tutorial Data")]
public class Tutorial : ScriptableObject
{
    public string about;
    
    [TextArea(3, 10)]
    public string[] sentences;
    public Sprite[] images;
}
