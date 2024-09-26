//using Cysharp.Threading.Tasks;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.ResourceManagement.AsyncOperations;
//using UnityEngine.AddressableAssets.ResourceLocators;
//using UnityEngine.AddressableAssets;
//using UnityEngine.ResourceManagement;
//using UnityEngine.ResourceManagement.ResourceLocations;
//using static RuntimeData;
//using UniRx;
//using System;


////public abstract class CatalogGenerator : MyButton
////{
////    public ContentsDatabase Database => ContentsDatabase.Databases[DLContentType];
////    public GameObject Flexalon => GameObject.Find(DisplayName).transform.Find("Grid Layout").gameObject;
////    public abstract DownloadContentType DLContentType { get; set; }
////    public abstract string DisplayName { get; set; }
////    public abstract string CatalogName { get; set; }
////    protected abstract UniTask CreateCatalog(string catalogName);
////    protected sealed override void Awake1()
////    {
////        GetComponent<MyUI>().On_Click.Subscribe(async _ => await CreateCatalog(CatalogName));
////    }
////}



//// �{�^���ɃA�^�b�`���Ƃ��āA�N���b�N������J�^���O�𐶐�����CV
////�iAddressable��catalog�ł͂Ȃ��A���[�U�[��DL�������R���e���c��I�Ԃ��߂̃J�^���O�I��UI�j
//public abstract class CatalogGeneratorCV : CV
//{
//    // �R���g���[���^�C�v��I��
//    public override Type ControllerType { get; set; } = typeof(MyButton);
//    // �v���[���^�[�^�C�v��I��
//    public override P Presenter { get; set; } // �Ȃ��H
    

//    // ���̎�ނ̃R���e���c�̃J�^���O�Ȃ̂��w�肷��
//    public abstract DownloadContentType DLContentType { get; set; }
//    // ContentsDatabase���ɁA�R���e���c��ނ��Ƃ̃f�[�^�x�[�X�C���X�^���X���Ǘ����鎫��������
//    public ContentsDatabase Database => ContentsDatabase.Databases[DLContentType];
//    // �Ȃ񂾂����\������J�^���O�̖��O�H
//    public abstract string DisplayName { get; set; }
//    // �Ȃ񂾂����J�^���O�̖{���H
//    public abstract string CatalogName { get; set; }
    

//    // DL�O�̃R���e���c�ł���̃Q�[���I�u�W�F�N�g�B���J�^���O�Ƃ��ĕ��ׂ邽�߂�Flexalon�����񂩂�擾
//    public GameObject Flexalon => GameObject.Find(DisplayName).transform.Find("Grid Layout").gameObject;
    

//    // �J�^���O�����̊֐��B�q�N���X�i�R���e���c�̎�ނ��Ƃɔh���j���Ď���
//    protected abstract UniTask CreateCatalog(string catalogName);


//    protected sealed override async UniTask Awake1()
//    {
//        // �J�^���O�����̃R���g���[�����N���b�N���ɃJ�^���O���������s
//        Controller.On_Click.Subscribe(async _ => await CreateCatalog(CatalogName));
        
//        await Awake2();
//    }
//    protected virtual async UniTask Awake2() { }
//}



//// �v���n�u�R���e���c�̃J�^���O�𐶐�
//// �ڂ����͐e�N���X���Q��
//public class CatalogGeneratorCV_Prefab : CatalogGeneratorCV
//{
//    // �I�[�o�[���C�h����v���p�e�B�ɂ��Ă͐e�N���X���Q�Ƃ��ꂽ��
//    public override DownloadContentType DLContentType { get; set; } = DownloadContentType.Prefab;
//    public override string DisplayName { get; set; } = "Catalog_Prefab";
//    public override string CatalogName { get; set; } = "�J�^���O1";


//    // �J�^���O����
//    protected override async UniTask CreateCatalog(string catalogName)
//    {
//        // CatalogModel�FCatalog �t�@�C���ƁA���̒��̃R���e���c�A�h���X������ۗL����
//        CatalogModel catalogModel = await Database.LoadCatalog(catalogName);

//        // �Ƃ���J�^���O���f���́A�e�A�h���X�����ɑS�ẴR���e���c�𐶐����Ă���
//        foreach (var a in catalogModel.Addresses)
//        {
//            // �A�h���X���琶��
//            GameObject content = await Addressables.InstantiateAsync(a);
//            // ���������R���e���g��Flexalon�̎q�I�u�W�F�N�g�ɂ��� 
//            content.transform.SetParent(Flexalon.transform);
//            // 
//            CatalogContentHeaderRV catalogContentCV = content.AddComponent<CatalogContentHeaderRV>();
//            catalogContentCV.Address = a;
//            catalogContentCV.CatalogFile = catalogModel.CatalogFile;
//            catalogContentCV.DLContentType = DownloadContentType.Prefab;

//            // �R���g���[���ƃv���[���^�̐ڑ�
//            //UIPresenterBase.AddGeneratedUI(content, new ImportContentPresenter(catalogContentController));

//            // ���ɃC���|�[�g����Ă���R���e���g�ɂ̓}�[�J�[������
//            foreach (var b in Database.importedContents.Values)
//            {
//                if (b.Address != a) continue;
//                GameObject Marker_IsImported = await Addressables.InstantiateAsync("Marker_IsImported");
//                Marker_IsImported.transform.SetParent(content.transform);
//                Marker_IsImported.transform.localPosition = Vector3.zero;
//            }
//        }
//    }
//}
