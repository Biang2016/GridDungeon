using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelConfigContainer : MonoBehaviour
{
    [TableList(ShowIndexLabels = true)]
    public List<LevelConfig> LevelConfigList;
}