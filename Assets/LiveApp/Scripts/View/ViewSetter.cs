using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System;
using static RuntimeData;
using Cysharp.Threading.Tasks;
using System.Windows.Forms;
#if UNITY_EDITOR
using UnityEditor;
#endif


// 複数のゲームオブジェクトに同じコンポーネントを使いたい
// 現状のシーン構成に依存するものと、iVの繋ぎ系の処理を管理する
// 初期化のタイミングを、iV に対していい感じにしたいので、InteractiveView を継承して Awake0() を使う

// コンポーネントのインスタンスは、アタッチすることで生成するので、コンストラクタは使えない。
// したがって複数のゲームオブジェクトに同じコンポーネントを流用し、かつ変数を異ならせたい場合、次の3つの選択肢がある。
//【1.インスペクターから設定】【2.他のスクリプトで後から設定】【3.流用は諦めて別のスクリプトとして複数種類のコンポーネントを作る】
// 全然異なるゲームオブジェクトに付けた同じコンポーネントの変数を、ゲームの場面に応じてどこかのスクリプトから変更することは不自然ではないが、
// UI要素のように複数の同じようなゲームオブジェクトの同じコンポーネントの変数を、手動で変更するのは面倒だし、場合によっては設定がリセットされる。
// また、変数の中身が違うだけでコンポーネントをいちいち別スクリプトとして作るわけにはいかない。
// よってこのような一括管クラスが必要となった。

//ただし、Buttonコンポーネントのように超いろんな箇所で流用されるコンポーネントはともかく２～３箇所くらいでしかく使われないものに関しては、それぞれのコンポーネントの初期化処理で条件分岐して値を異ならせるのでも十分な気がする。例えば自ゲームオブジェクトの名前が◯◯だったら～とか。

//[ExecuteInEditMode]
public class ViewSetter : MonoBehaviour
{
    [SerializeField]
    RuntimeAnimatorController animController_Button;

    public static BoolReactiveProperty Initialized { get; set; } = new BoolReactiveProperty(false);

    // IV の Initialized を待てば、IV継承かつAwake0()　じゃなくても良い気がするので試す。
    //protected sealed override async UniTask Awake0()
    //{
    //    ApplyAnim(animController_Button, "UI");
    //    ApplyAnim(animController_Button, "TestButton");
    //    ApplySlider(GameObject.Find("Slider_SizeAdjuster_Avatar"), _Avatar);
    //    ApplySlider(GameObject.Find("Slider_SizeAdjuster_Room"), Room);
    //}
    private void Awake()
    {
        ApplyAnim(animController_Button, "UI_Under");
        //ApplyAnim(animController_Button, "TestButton");
        
        //ここはコンポーネント自身内の初期化処理に担わせることにした。
        //ApplySlider(GameObject.Find("Slider_SizeAdjuster_Avatar"), _Avatar);
        //ApplySlider(GameObject.Find("Slider_SizeAdjuster_Room"), Room);

        Initialized.Value = true;
    }





    //UI各要素で使うアニメーション(マウスオーバー時等)を一括で設定。
    //UI要素の親オブジェクトの名前をと、設定したいアニメーターコントローラを引数に渡すと、子UI要素オブジェクトに一括でアニメーションをつけてくれる
    async void ApplyAnim(RuntimeAnimatorController animatorController, string targetName)
    {
        CV[] views = GameObject.Find(targetName).GetComponentsInChildren<CV>();
        foreach (CV view in views)
        {
            await UniTask.WaitUntil(() => view.Controller != null);
            MyButton controller = view.Controller as MyButton;
            if (controller == null) continue;
            controller.animatorController.Value = animatorController;
        }
    }




    ////アプリの構成オブジェクトにアタッチされているサイズ更新用コンポ SizeReflector と、サイズ操作用のスライダーオブジェクトを繋げる
    //async void ApplySlider(GameObject sliderUI, GameObject target)
    //{
    //    SizeAdjusterRV view = target.GetComponent<SizeAdjusterRV>();
    //    if (view.ControllerObj == null) view.ControllerObj = sliderUI;
    //}



//    protected override sealed void Update()
//    {
//#if UNITY_EDITOR
//        if (!EditorApplication.isPlaying)
//        {
//            Awake();
//        }
//#endif
//    }
}