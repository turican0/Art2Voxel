using Godot;
using System;

public partial class VSlider1TE : TextEdit
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Text = "0";
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_v_slider_1_value_changed(double value)
	{
        Text = value.ToString();
        TextureRect node = (TextureRect)GetNode("../../../SubViewportContainer/SubViewport/MyTextureRectVox/MyTextureRect");
        if (GetNode("../..").Name == "VSlider1")
            node.ChangeAddPosY((int)value);
        else
            node.ChangeScaleH((float)value);
    }
}



