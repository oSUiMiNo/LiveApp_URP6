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
        
        // ���I�u�W�F�N�g�̃��b�V���팸
        DecimateMesh(transform);
        // �q���I�u�W�F�N�g�̃��b�V���팸
        DecimateMesh_Descendants(transform);
    }


    void Detect_SkeltonRoots(Transform trans)
    {
        // SkinnedMeshRenderer�̑��݊m�F
        SkinnedMeshRenderer skinnedRenderer = trans.gameObject.GetComponent<SkinnedMeshRenderer>();
        // SkinnedMeshRenderer��������΃X�L�b�v
        if (!skinnedRenderer) return;
        // �X�P���g���̃��[�g�̓��X�g�ɓ����
        if (!RootObjs.Contains(skinnedRenderer.rootBone)) RootObjs.Add(skinnedRenderer.rootBone);
    }

    void Detect_VRMSpringBone(Transform trans)
    {
        // VRMSpringBone�̑��݊m�F
        VRMSpringBone[] springBones = trans.gameObject.GetComponents<VRMSpringBone>();
        // VRMSpringBone��������΃X�L�b�v
        if (springBones.Length == 0) return;
        // VRMSpringBone�������[�u
        foreach (var a in springBones) Destroy(a);
    }

    void DecimateMesh_Descendants(Transform trans)
    {
        foreach (Transform child in transform)
        {
            // VRM�̃X�P���g���̃��[�g��������X�L�b�v
            if (RootObjs.Contains(trans)) return;

            // ���b�V���팸
            DecimateMesh(child);

            // ����Ɏq�I�u�W�F�N�g�������Ă���΍ċN����
            if (child.childCount != 0) DecimateMesh_Descendants(child);
        }
    }

    void DecimateMesh(Transform trans)
    {
        // MeshFilter�̑��݊m�F
        MeshFilter filter = trans.GetComponent<MeshFilter>();
        // SkinnedMeshRenderer�̑��݊m�F
        SkinnedMeshRenderer skinnedRenderer = trans.GetComponent<SkinnedMeshRenderer>();

        float quality = 1;
        // MeshFilter��SkinnedMeshRenderer������΃��b�V���팸
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
    //    // �w��I�u�W�F�N�g�̑S�Ă̎q�I�u�W�F�N�g���`�F�b�N����
    //    foreach (Transform child in transform)
    //    {
    //        // VRM�̃X�P���g���̃��[�g��������X�L�b�v
    //        if (RootObjs.Contains(child)) continue;

    //        // MeshFilter�̑��݊m�F
    //        MeshFilter filter = child.GetComponent<MeshFilter>();
    //        // SkinnedMeshRenderer�̑��݊m�F
    //        SkinnedMeshRenderer skinnedRenderer = child.gameObject.GetComponent<SkinnedMeshRenderer>();

    //        // MeshFilter��SkinnedMeshRenderer������΃��b�V���팸
    //        if (filter)
    //        {
    //            DecimateMesh_MeshFilter(filter);
    //            Debug.Log($"�t�B���^ {filter.gameObject.name}");
    //        }
    //        else
    //        if (skinnedRenderer)
    //        {
    //            DecimateMesh_SkinnedMeshRenderer(skinnedRenderer);
    //            Debug.Log($"�X�L���h {skinnedRenderer.gameObject.name}");
    //        }

    //        // ����Ɏq�I�u�W�F�N�g�������Ă���΍ċN����
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
