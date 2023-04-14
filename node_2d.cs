using Art2Voxel;
using Godot;
using System;
using System.IO;
//using System.IO; //for Get directory

public partial class node_2d : Node2D
{

    private void AddRotTo(string optionPath, ImgStr imgStr)
    {
        OptionButton nodeButt = (OptionButton)this.GetNode(optionPath);
        nodeButt.AddItem(imgStr.rotation.ToString());
    }
    private void AddRotationOptions()
    {
        foreach (ImgStr imgStr in VoxelClass.listTexture)
        {
            AddRotTo("./Window/OptionButton", imgStr);
            AddRotTo("./Window2/OptionButton", imgStr);
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        VoxelClass.Init();
        VoxelClass.AnalyzeFolder("res://img/");
        AddRotationOptions();
        VoxelClass.CreateVoxelArray();
        VoxelClass.FillByImage(0,1);
        VoxelClass.inited=true;
        TextureRect textNode = (TextureRect)GetNode("Window/SubViewportContainer/SubViewport/MyTextureRectVox/MyTextureRect");
        textNode.UpdatePos();
        TextureRect textNode2 = (TextureRect)GetNode("Window2/SubViewportContainer/SubViewport/MyTextureRectVox/MyTextureRect");
        textNode2.UpdatePos();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
