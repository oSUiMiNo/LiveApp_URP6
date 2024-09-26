using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


public abstract class EmptyP : P{ }




public abstract class P
{
    public ReactiveProperty<bool> Clicked = new ReactiveProperty<bool>(false);

    public FloatReactiveProperty value_Float = new FloatReactiveProperty(0);
    public IntReactiveProperty value_Int = new IntReactiveProperty(0);

    public StringReactiveProperty _Text = new StringReactiveProperty(string.Empty);

    public static Dictionary<GameObject, P> Children = new Dictionary<GameObject, P>();


    public abstract UniTask Execute();
    public async virtual UniTask Desist() { }
}



public abstract class P_Singleton<T> : Singleton<T> 
    where T : Singleton<T>, new()
{
    public ReactiveProperty<bool> Clicked = new ReactiveProperty<bool>(false);

    public FloatReactiveProperty value_Float = new FloatReactiveProperty(0);
    public IntReactiveProperty value_Int = new IntReactiveProperty(0);

    public StringReactiveProperty _Text = new StringReactiveProperty(string.Empty);


    public static Dictionary<GameObject, P> Children = new Dictionary<GameObject, P>();

    

    public abstract UniTask Execute();
    public async virtual UniTask Desist() { }
}



