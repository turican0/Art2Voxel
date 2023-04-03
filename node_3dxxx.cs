using Godot;
using System;

public partial class node_3dxxx : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        // Vytvo�en� nov� instance BoxMesh.
        var boxMesh = new BoxMesh();

        // Vytvo�en� nov� instance SpatialMaterial s nastaven�m zelen�m barvou.
        var material = new StandardMaterial3D();
        material.AlbedoColor = Colors.Green;

        // Vytvo�en� nov� instance MeshInstance a nastaven� BoxMesh a SpatialMaterial.
        var meshInstance = new Godot.MeshInstance3D();
        meshInstance.Mesh = boxMesh;
        meshInstance.MaterialOverlay = material;

        // P�id�n� MeshInstance do sc�ny.
        AddChild(meshInstance);

        //AddChild(new Node3DVoxel());
        //GetViewport().ClearColor = new Color(1, 1, 1);
        /*
		 Material material = new SpatialMaterial();
material.AlbedoColor = new Color(1, 1, 1);
material.ParamsAmbientLight = 0;
material.ParamsDiffuseLight = 0;
material.ParamsSpecularLight = 0;
		*/
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
