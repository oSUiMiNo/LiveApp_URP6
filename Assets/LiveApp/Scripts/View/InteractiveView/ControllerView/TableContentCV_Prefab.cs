//using Cysharp.Threading.Tasks;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UniRx;
//using System;


//// =================���ȍ~�́A�����܂��g���Ă��Ȃ� ========================



//public abstract class TableContentCV : CV
//{
//    // �R���g���[���^�C�v��I��
//    public override Type ControllerType { get; set; } = typeof(MyButton);
//    // �v���[���^�[�^�C�v��I��
//    public override P Presenter { get; set; } // �Ȃ��H


//    // ���̎�ނ̃R���e���c�̃e�[�u���Ȃ̂��w�肷��
//    public abstract DownloadContentType DLContentType { get; set; }
//    // ContentsDatabase���ɁA�R���e���c��ނ��Ƃ̃f�[�^�x�[�X�C���X�^���X���Ǘ����鎫��������
//    public ContentsDatabase Database => ContentsDatabase.Databases[DLContentType];
//    // �R���e���g�̖��O�H�Ȃ�ŁH
//    public string ContentName => gameObject.name;
    
    
//    protected abstract UniTask DischargeOwnContent();
//    protected sealed override async UniTask Awake1()
//    {
//        //GetComponent<MyUI>().On_Click.Subscribe(async _ => await DischargeOwnContent());
//    }
//}



//public class TableContentCV_Prefab : TableContentCV
//{
//    public override DownloadContentType DLContentType { get; set; } = DownloadContentType.Prefab;


//    // ���L���Ă���R���e���g����o
//    protected override async UniTask DischargeOwnContent()
//    {
//        GameObject content = Instantiate(gameObject, Vector3.zero, Quaternion.identity);
//        Destroy(content.GetComponent<MyUI>());
//        Destroy(content.GetComponent<TableContentCV_Prefab>());
//    }
//}