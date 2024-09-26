using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using VRM;

public class MyMeshOptimizer_VRM : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    [SerializeField] float _quality = 0.35f;
    
    List<Transform> RootObjs = new List<Transform>();
   
    void Start()
    {
        Debug.Log(gameObject);

        Detect_SkeltonRoots(transform);
        foreach (Transform child in transform) Detect_SkeltonRoots(child);
        
        Detect_VRMSpringBone(transform);
        foreach (Transform child in transform) Detect_VRMSpringBone(child);
        
        // 自オブジェクトのメッシュ削減
        DecimateMesh(transform);
        // 子孫オブジェクトのメッシュ削減
        DecimateMesh_Descendants(transform);
    }


    void Detect_SkeltonRoots(Transform trans)
    {
        // SkinnedMeshRendererの存在確認
        SkinnedMeshRenderer skinnedRenderer = trans.gameObject.GetComponent<SkinnedMeshRenderer>();
        // SkinnedMeshRendererが無ければスキップ
        if (!skinnedRenderer) return;
        // スケルトンのルートはリストに入れる
        if (!RootObjs.Contains(skinnedRenderer.rootBone)) RootObjs.Add(skinnedRenderer.rootBone);
    }

    void Detect_VRMSpringBone(Transform trans)
    {
        // VRMSpringBoneの存在確認
        VRMSpringBone[] springBones = trans.gameObject.GetComponents<VRMSpringBone>();
        // VRMSpringBoneが無ければスキップ
        if (springBones.Length == 0) return;
        // VRMSpringBoneをリムーブ
        foreach (var a in springBones) Destroy(a);
    }

    void DecimateMesh_Descendants(Transform trans)
    {
        foreach (Transform child in transform)
        {
            // VRMのスケルトンのルートだったらスキップ
            if (RootObjs.Contains(trans)) return;

            // メッシュ削減
            DecimateMesh(child);

            // さらに子オブジェクトを持っていれば再起処理
            if (child.childCount != 0) DecimateMesh_Descendants(child);
        }
    }

    void DecimateMesh(Transform trans)
    {
        // MeshFilterの存在確認
        MeshFilter filter = trans.GetComponent<MeshFilter>();
        // SkinnedMeshRendererの存在確認
        SkinnedMeshRenderer skinnedRenderer = trans.GetComponent<SkinnedMeshRenderer>();

        float quality = 1;
        // MeshFilterかSkinnedMeshRendererがあればメッシュ削減
        if (filter)
        {
            Mesh mesh = filter.sharedMesh;
            if (mesh.vertexCount <= 80) return;
            else
            if (mesh.vertexCount <= 3000) quality = 0.4f;
            else
            if (mesh.vertexCount <= 6000) quality = 0.3f;
            else
            if (mesh.vertexCount <= 15000) quality = 0.2f;
            else quality = 0.1f;

            filter.sharedMesh = GetDecimatedMesh(mesh, quality);
        }
        else
        if (skinnedRenderer)
        {
            Mesh mesh = skinnedRenderer.sharedMesh;
            if (mesh.vertexCount <= 80) return;
            else
            if (mesh.vertexCount <= 3000) quality = 0.4f;
            else
            if (mesh.vertexCount <= 6000) quality = 0.3f;
            else
            if (mesh.vertexCount <= 15000) quality = 0.2f;
            else quality = 0.1f;

            skinnedRenderer.sharedMesh = GetDecimatedMesh(mesh, quality);
        }
    }

    public Mesh GetDecimatedMesh(Mesh mesh, float quality)
    {
        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        meshSimplifier.Initialize(mesh);
        meshSimplifier.SimplifyMesh(quality);
        Mesh destMesh = meshSimplifier.ToMesh();
        if(destMesh.vertexCount == 0) destMesh = mesh;
        return destMesh;
    }


    //void DecimateDescendantsMesh(Transform transform)
    //{
    //    // 指定オブジェクトの全ての子オブジェクトをチェックする
    //    foreach (Transform child in transform)
    //    {
    //        // VRMのスケルトンのルートだったらスキップ
    //        if (RootObjs.Contains(child)) continue;

    //        // MeshFilterの存在確認
    //        MeshFilter filter = child.GetComponent<MeshFilter>();
    //        // SkinnedMeshRendererの存在確認
    //        SkinnedMeshRenderer skinnedRenderer = child.gameObject.GetComponent<SkinnedMeshRenderer>();

    //        // MeshFilterかSkinnedMeshRendererがあればメッシュ削減
    //        if (filter)
    //        {
    //            DecimateMesh_MeshFilter(filter);
    //            Debug.Log($"フィルタ {filter.gameObject.name}");
    //        }
    //        else
    //        if (skinnedRenderer)
    //        {
    //            DecimateMesh_SkinnedMeshRenderer(skinnedRenderer);
    //            Debug.Log($"スキンド {skinnedRenderer.gameObject.name}");
    //        }

    //        // さらに子オブジェクトを持っていれば再起処理
    //        if (transform.childCount != 0) DecimateDescendantsMesh(child);
    //    }
    //}


    //public Mesh DecimateMesh_MeshFilter(MeshFilter filter)
    //{
    //    Mesh mesh = filter.sharedMesh;
    //    var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
    //    meshSimplifier.Initialize(mesh);
    //    meshSimplifier.SimplifyMesh(_quality);
    //    //Mesh destMesh = meshSimplifier.ToMesh();
    //    return meshSimplifier.ToMesh();
    //    //filter.sharedMesh = destMesh;
    //}

    //public Mesh DecimateMesh_SkinnedMeshRenderer(SkinnedMeshRenderer skinnedMeshRenderer)
    //{
    //    Mesh mesh = skinnedMeshRenderer.sharedMesh;
    //    var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
    //    meshSimplifier.Initialize(mesh);
    //    meshSimplifier.SimplifyMesh(_quality);
    //    //Mesh destMesh = meshSimplifier.ToMesh();
    //    return meshSimplifier.ToMesh();
    //    //skinnedMeshRenderer.sharedMesh = destMesh;
    //}
}
