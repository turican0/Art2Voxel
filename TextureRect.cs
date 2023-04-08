using Art2Voxel;
using Godot;
using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using static Godot.RenderingServer;
using static System.Net.Mime.MediaTypeNames;

[Tool]
public partial class TextureRect : Godot.TextureRect{
    private bool first_run = true;
    private Vector2 chPos;
    private Vector2 origPos;
    private Vector2 origScale;
    private Vector2 chScale;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {      
        StartRun();
    }

    private void StartRun()
    {
        if(Name== "MyTextureRect")
            if (first_run)
            {
                first_run = false;

                string[] files = Directory.GetFiles(ProjectSettings.GlobalizePath("res://img/"), "*.png");
                LoadImageAsTexture(files[0]);
                //loadB();
                //SetGrid();
            }
    }

    /*public void loadB()
    {
        // Vytvoření nové textury 100x100
        ImageTexture texture = new ImageTexture();
        Godot.Image image = Godot.Image.Create(100, 100, false, Godot.Image.Format.Rgb8);
        // Nakreslení šikmé čáry
        var color = new Godot.Color(1, 1, 1);
        for (int i = 0; i < 100; i++)
        {
            image.SetPixel(i, i, color);
        }
        texture = ImageTexture.CreateFromImage(image);
        // Použití textury jako zdroje pro zobrazení TextureRect
        Texture = texture;
    }*/
    /*
    private void SetGrid()
    {
        int scale = 10;
        TextureRect childGridMode = new TextureRect();
        childGridMode.Name = "MyGridTextureRect";
        Vector2 rectSize = Texture.GetSize();
        Vector2 currentScale = Scale;
        //currentScale.X = 1.0f/ scale;
        //currentScale.Y = 1.0f/ scale;
        currentScale.X = 1.0f / scale;// * (((rectSize.X + 1) * scale) / (float)((rectSize.X * scale) + 1));
        currentScale.Y = 1.0f / scale;// * (((rectSize.X + 1) * scale) / (float)((rectSize.Y * scale) + 1));
        childGridMode.Scale = currentScale;
        ImageTexture texture = new ImageTexture();
        Godot.Image image = Godot.Image.Create(1+(int)(rectSize.X* scale), 1+(int)(rectSize.Y* scale), false, Godot.Image.Format.Rgba8);
        //var color = new Godot.Color(1, 1, 1, 1);
        for (int y = 0; y < 1+rectSize.Y* scale; y++)
            for (int x = 0; x < 1+rectSize.X* scale; x += scale)
                image.SetPixel(x, y, new Godot.Color(0, 0, 0, 1));
        for (int y = 0; y < 1+rectSize.Y* scale; y += scale)
            for (int x = 0; x < 1+rectSize.X* scale; x++)
                image.SetPixel(x, y, new Godot.Color(0, 0, 0, 1));
        texture = ImageTexture.CreateFromImage(image);
        childGridMode.Texture = texture;
        childGridMode.TextureFilter = TextureFilterEnum.Nearest;
        AddChild(childGridMode);
    }
    */

    private void LoadImageAsTexture(string imagePath)
    {
        Texture2D icon = ResourceLoader.Load(imagePath) as Texture2D;

        Texture = icon;
        TextureFilter = TextureFilterEnum.Nearest;
        Size = icon.GetSize();
        /*float newScale = (400f / Size.X);
        if(newScale>256)
            newScale = 256;
        else if (newScale > 128)
            newScale = 128;
        else if (newScale > 64)
            newScale = 64;
        else if (newScale > 32)
            newScale = 32;
        else if (newScale > 16)
            newScale = 16;
        else if (newScale > 8)
            newScale = 8;
        else if (newScale > 4)
            newScale = 4;
        else if (newScale > 2)
            newScale = 2;
        else newScale = 1;*/
        //int newScale = 8;
        //Scale = new Vector2(newScale, newScale);

        //Vector2 voxelSize = VoxelClass.GetSize();
        //Position = (Size - voxelSize) / 2;
        int newScale = 3;
        Scale = new Vector2(newScale, newScale);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
            StartRun();
    }
    internal void UpdatePos()
    {
        
        Vector2 voxelSize = VoxelClass.GetSize();
        Position = new Vector2(3 * (int)(-(Size.X - voxelSize.X) / 2 - 0.5f), 3 *(int)(- (Size.Y - voxelSize.Y) / 2 -0.5f));
        origPos = Position;
        origScale = Scale;
        chScale = new Vector2(1, 1);
        ////MyTextureRectVox myTextureRectVox = (MyTextureRectVox)GetNode("../../MyTextureRectVox");
        ////myTextureRectVox.Position = new Vector2(-Position.X + 0.1f, -Position.Y + 0.1f);        
    }

    public void _on_option_button_item_selected2(long index)
    {
        string[] files = Directory.GetFiles(ProjectSettings.GlobalizePath("res://img/"), "*.png");
        LoadImageAsTexture(files[index]);
        UpdatePos();
    }

    internal void ChangeAddPosX(int value)
    {
        chPos = new Vector2(value, chPos.Y);
        Position = origPos + chPos * 3;
    }

    internal void ChangeAddPosY(int value)
    {
        chPos = new Vector2(chPos.X, value);
        Position = origPos + chPos * 3;
    }

    internal void ChangeScaleW(float value)
    {
        chScale = new Vector2(value, chScale.Y);
        Scale = origScale * chScale;
    }

    internal void ChangeScaleH(float value)
    {
        chScale = new Vector2(chScale.X, value);
        Scale = origScale * chScale;
    }
}
