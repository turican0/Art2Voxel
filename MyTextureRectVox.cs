using Art2Voxel;
using Godot;
using System;

public partial class MyTextureRectVox : TextureRect
{
	private bool first_run = true;
    private float lastRotation;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        //StartRun();
    }

	public void StartRun()
	{
		if (first_run)
		{
			if (VoxelClass.inited)
			{
				first_run = false;
				InitImage();
				DrawVoxel(0);
				SetGrid();
            }
		}
	}

    private void SetGrid()
    {
        int scale = 10;
        TextureRect childGridMode = new TextureRect();
        childGridMode.Name = "MyGridTextureRect";
        Vector2 rectSize = Texture.GetSize();
        Vector2 currentScale = Scale;
        //currentScale.X = 1.0f/ scale;
        //currentScale.Y = 1.0f/ scale;
        currentScale.X = 3 * 1.0f / scale;// * (((rectSize.X + 1) * scale) / (float)((rectSize.X * scale) + 1));
        currentScale.Y = 3 * 1.0f / scale;// * (((rectSize.X + 1) * scale) / (float)((rectSize.Y * scale) + 1));
        childGridMode.Scale = currentScale;
        ImageTexture texture = new ImageTexture();
        Godot.Image image = Godot.Image.Create(1 + (int)(rectSize.X/3 * scale), 1 + (int)(rectSize.Y/3 * scale), false, Godot.Image.Format.Rgba8);
        var color = new Godot.Color(255, 255, 255, 255);
        for (int y = 0; y < 1 + rectSize.Y/3 * scale; y++)
            for (int x = 0; x < 1 + rectSize.X/3 * scale; x += scale)
                image.SetPixel(x, y, new Godot.Color(0, 0, 0, 255));
        for (int y = 0; y < 1 + rectSize.Y/3 * scale; y += scale)
            for (int x = 0; x < 1 + rectSize.X/3 * scale; x++)
                image.SetPixel(x, y, new Godot.Color(0, 0, 0, 255));
        texture = ImageTexture.CreateFromImage(image);
        childGridMode.Texture = texture;
        childGridMode.TextureFilter = TextureFilterEnum.Nearest;
        AddChild(childGridMode);
    }

    public void InitImage()
	{
		ImageTexture texture = new ImageTexture();
		Godot.Image image = Godot.Image.Create(VoxelClass.maxXY*3, VoxelClass.maxZ*3, false, Godot.Image.Format.Rgba8);
		//for (int i = 0; i < 50; i++)
		//    image.SetPixel(i, i, new Godot.Color(255, 0, 0, 255));
		texture = ImageTexture.CreateFromImage(image);
		Texture = texture;
		TextureFilter = TextureFilterEnum.Nearest;

        Size = texture.GetSize() / 3;

		float newScale = 1f / 3;
        Scale = new Vector2(newScale, newScale);
    }

	public void DrawVoxel(float rotation)
	{
		lastRotation = rotation;
		Godot.Color[,] imgCopy = VoxelClass.GetImage(rotation);
		Image image = Texture.GetImage();

        Godot.Color transColor = new Godot.Color(0,1,0,1);

        for (int y = 0; y < image.GetHeight() / 3; y++)
			for (int x = 0; x < image.GetWidth() / 3; x++)
			{
				if (imgCopy[y, x].A == 0)
				{
                    image.SetPixel(x * 3 + 0, y * 3, transColor);
                    image.SetPixel(x * 3 + 1, y * 3, transColor);
                    image.SetPixel(x * 3 + 2, y * 3, transColor);
                }
				else
				{
					image.SetPixel(x * 3 + 0, y * 3, imgCopy[y, x]);
					image.SetPixel(x * 3 + 1, y * 3, imgCopy[y, x]);
					image.SetPixel(x * 3 + 2, y * 3, imgCopy[y, x]);
				}
            }
				//image.SetPixel(x, y, new Godot.Color(255, 0, 0, 255));
		Texture = ImageTexture.CreateFromImage(image);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		StartRun();		
	}
	
	private void _on_option_button_item_selected(long index)
	{
		DrawVoxel(-(index*22.5f));
        TextureRect parentNode = (TextureRect)GetNode("../../MyTextureRect");
		parentNode._on_option_button_item_selected2(index);

    }

    internal void DrawVoxel()
    {
        DrawVoxel(lastRotation);
    }
}



