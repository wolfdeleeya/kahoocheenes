using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UnityEventString : UnityEvent<string>
{
}

[Serializable]
public class UnityEventFloat : UnityEvent<float>
{
}


[Serializable]
public class UnityEventInteger : UnityEvent<int>
{
}

[Serializable]
public class UnityEventControlsPair : UnityEvent<ControlsPair>
{
}

[Serializable]
public class UnityEventGameObject : UnityEvent<GameObject>
{
}