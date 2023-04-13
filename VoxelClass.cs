﻿using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Godot.HttpRequest;

namespace Art2Voxel
{
    public struct ImgStr
    {  
        public Texture2D texture2D;
        public String name;
        public float rotation;
        public Vector2 size;
    }
    internal static class VoxelClass
    {
        public static int maxXY = 0;
        public static int maxZ = 0;
        public static bool inited = false;
        static Godot.Color[,,] voxArray;
        static List<ImgStr> listTexture;//+rotation

        public static void Init()
        {
            listTexture = new List<ImgStr>();
        }

        public static ImgStr CreateImgStr(string filePath)
        {
            ImgStr imgstr=new ImgStr();

            Texture2D icon = ResourceLoader.Load(filePath) as Texture2D;
            imgstr.texture2D=icon;

            imgstr.size.X = icon.GetWidth();
            imgstr.size.Y = icon.GetHeight();

            imgstr.name= filePath;

            char[] delimiterChars = { '.' };            

            string[] words = filePath.Split(delimiterChars);

            string str1 = words[words.Length - 2];

            str1 = str1.Replace(",", ".");

            //imgstr.rotation = (float)float.Parse(str1, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            //var d = Convert.ToDecimal("1.2345", new CultureInfo("en-US"));
            imgstr.rotation = (float)Convert.ToDouble(str1, new NumberFormatInfo() { NumberDecimalSeparator = "." });
            return imgstr;
        }

        public static Vector2 GetPNGSize(string filePath)
        {
            //Image image = new Image();
            //Texture2D icon = ResourceLoader.Load(filePath) as Texture2D;
            ImgStr imgStr = CreateImgStr(filePath);
            listTexture.Add(imgStr);
            Vector2 result;
            result = imgStr.size;
            return result;
        }
        public static void AnalyzeImage(string file)
        {
            Vector2 pngSize = GetPNGSize(file);
            if (pngSize.X > maxXY)
                maxXY = (int)pngSize.X;
            if (pngSize.Y > maxZ)
                maxZ = (int)pngSize.Y;
        }

        public static void CreateVoxelArray()
        {
            voxArray = new Godot.Color[maxZ, maxXY, maxXY];
        }

        public static void FillByImage(int imgIndex, int size)
        {
            //Image image = new Image();
            Texture2D icon = listTexture[imgIndex].texture2D;
            Image image = icon.GetImage();
            if (size > maxXY)
                size = maxXY;

            int imgWidth = image.GetWidth();
            int imgHeight = image.GetHeight();

            int addXY = (maxXY - imgWidth) / 2;
            int addZ = (maxZ - imgHeight) / 2;

            int addSXY = (maxXY - size) / 2;
            //int addSY = (maxZ - size) / 2;

            Godot.Color[,] imgCopy = new Godot.Color[maxZ, maxXY];
            for (int y = 0; y < imgHeight; y++)
                for (int x = 0; x < imgWidth; x++)
                {
                    imgCopy[y + addZ, x + addXY] = image.GetPixel(x, y);
                    //GD.Print(x + "+" + y);
                }
            int Xmin = addXY;
            if (Xmin < 0) Xmin = 0;
            int Xmax = imgWidth + addXY;
            if (Xmax > maxXY) Xmax = maxXY;

            int Ymin = addSXY;
            if (Ymin < 0) Ymin = 0;
            int Ymax = size + addSXY;
            if (Ymax > maxXY) Ymax = maxXY;

            for (int x = Xmin; x < Xmax; x++)
                for (int y = Ymin; y < Ymax; y++)
                    for (int z = 0; z < maxZ; z++)
                    {
                        //if (imgCopy[z, x] == new Godot.Color(0.5019608f, 0.5019608f, 0.5019608f, 1))
                        //    imgCopy[z, x].A = 0;
                        voxArray[z, y, x] = imgCopy[z, x];
                        //voxArray[x, y, z] = image.GetPixel(x,z);
                        //voxArray[x, y, z].R = imgData[(x + z * image.GetWidth())*2 + 0] / (float)256;// image.GetPixel(x,z);
                        //voxArray[x, y, z].G = imgData[(x + z * image.GetWidth()) * 4 + 1] / (float)256;// image.GetPixel(x,z);
                        //voxArray[x, y, z].B = imgData[(x + z * image.GetWidth()) * 4 + 2] / (float)256;// image.GetPixel(x,z);
                        //voxArray[x, y, z].A = imgData[(x + z * image.GetWidth()) * 4 + 3] / (float)256;// image.GetPixel(x,z);
                    }
        }

        static public void AnalyzeFolder(string directoryPath)
        {
            //string[] files = Directory.GetFiles(directoryPath);
            string[] files = Directory.GetFiles(ProjectSettings.GlobalizePath(directoryPath), "*.png");
            foreach (string file in files)
            {
                VoxelClass.AnalyzeImage(file);
            }
        }

        internal static Godot.Color[,] GetImage(double rotation)
        {
            Godot.Color[,] imgCopy = new Godot.Color[maxZ, maxXY];
            while (rotation < 0)
                rotation += 360;
            while (rotation > 360)
                rotation -= 360;
            double angle = Math.PI * rotation / 180.0;
            double s = Math.Sin(angle);
            double c = Math.Cos(angle);

            int maxXYZ = maxXY * maxZ;
            int zoom = 1;

            int minX = 0;
            int maxX = maxXY;
            int addX = 1;

            int minY = 0;
            int maxY = maxXY;
            int addY = 1;
            if (rotation < 180)
            {
                minX = maxXY-1;
                maxX = -1;
                addX = -1;
            }
            if ((rotation > 90) && (rotation < 270))
            {
                minY = maxXY - 1;
                maxY = -1;
                addY = -1;
            }
            
            double tempAddCenter = ((maxXY - 1) * 0.5 * zoom);

            for (int x = minX; x != maxX; x += addX)
                for (int y = minY; y != maxY; y += addY)
                {
                    int newX = (int)(((x - tempAddCenter) * c - (y - tempAddCenter) * s) + tempAddCenter + 0.0001);
                    int newY = (int)(((x - tempAddCenter) * s + (y - tempAddCenter) * c) + tempAddCenter + 0.0001);
                    if ((newX < 0) || (newY < 0))
                        continue;
                    if ((newX > maxXY - 1) || (newY > maxXY - 1))
                        continue;
                    //int tempIndex = newX * maxXYZ + newY * maxZ;
                    for (int z = 0; z < maxZ; z++)
                    {
                        if (voxArray[z, newY, newX].A == 1)
                        {
                            imgCopy[z, x] = voxArray[z, newY, newX];
                        }
                    }
                }
            return imgCopy;
        }

        internal static Vector2 GetSize()
        {
            return new Vector2(maxXY, maxZ);
        }

        internal static void Pressed(int pressed, Vector2 position, Vector2 scale, Vector2 size, Vector2 mouse)
        {
            Vector2 pressedPixel = ((mouse - position) / scale) / 3f;
            Vector2 pressedPixel2;
            pressedPixel2.X = pressedPixel.X;// + (int)((maxXY - size.X)/2);
            pressedPixel2.Y = pressedPixel.Y;// + (int)((maxZ - size.Y)/ 2);
            GD.Print("Pressed pixel:" + pressedPixel);
            int x = (int)pressedPixel2.X;
            int z = (int)pressedPixel2.Y;

            if (pressed == 2)
            {
                FindBest(z,x);
            }
            else
            {

                int lastVoxel = -1;
                for (int y = 0; y != maxXY; y += 1)
                {
                    if (voxArray[z, y, x].A == 1)
                    {
                        lastVoxel = y;
                        break;
                    }
                }
                if (pressed == 0)
                {
                    lastVoxel--;
                    if (lastVoxel >= 0)
                        voxArray[z, lastVoxel, x] = voxArray[z, lastVoxel + 1, x];
                }
                else
                {
                    if (lastVoxel >= 0)
                    {
                        if (lastVoxel + 1 < maxXY)
                        {
                            if (voxArray[z, lastVoxel + 1, x].A == 0)
                                voxArray[z, lastVoxel + 1, x] = voxArray[z, lastVoxel, x];
                        }
                        voxArray[z, lastVoxel, x] = new Godot.Color(0, 0, 0, 0);
                    }
                }
            }
            /*
        //for (int x = 0; x != maxXY; x += 1)
        for (int y = 0; y != maxXY; y += 1)
                //for (int z = 0; z < maxZ; z++)
                {
                    voxArray[z, y, x] = new Color(1,0,1,1);
                }*/
        }

        internal static int GetDiffAlphaImg(double rotation, Texture2D texture2D)
        {
            Godot.Color[,] imgCopy = new Godot.Color[maxZ, maxXY];
            while (rotation < 0)
                rotation += 360;
            while (rotation > 360)
                rotation -= 360;
            double angle = Math.PI * rotation / 180.0;
            double s = Math.Sin(angle);
            double c = Math.Cos(angle);

            int maxXYZ = maxXY * maxZ;
            int zoom = 1;

            int minX = 0;
            int maxX = maxXY;
            int addX = 1;

            int minY = 0;
            int maxY = maxXY;
            int addY = 1;
            if (rotation < 180)
            {
                minX = maxXY - 1;
                maxX = -1;
                addX = -1;
            }
            if ((rotation > 90) && (rotation < 270))
            {
                minY = maxXY - 1;
                maxY = -1;
                addY = -1;
            }

            double tempAddCenter = ((maxXY - 1) * 0.5 * zoom);

            for (int x = minX; x != maxX; x += addX)
                for (int y = minY; y != maxY; y += addY)
                {
                    int newX = (int)(((x - tempAddCenter) * c - (y - tempAddCenter) * s) + tempAddCenter + 0.0001);
                    int newY = (int)(((x - tempAddCenter) * s + (y - tempAddCenter) * c) + tempAddCenter + 0.0001);
                    if ((newX < 0) || (newY < 0))
                        continue;
                    if ((newX > maxXY - 1) || (newY > maxXY - 1))
                        continue;
                    //int tempIndex = newX * maxXYZ + newY * maxZ;
                    for (int z = 0; z < maxZ; z++)
                    {
                        if (voxArray[z, newY, newX].A == 1)
                        {
                            imgCopy[z, x] = voxArray[z, newY, newX];
                        }
                    }
                }
            //return imgCopy;

            Image image = texture2D.GetImage();

            //if (size > maxXY)
            //int size = maxXY;

            int imgWidth = image.GetWidth();
            int imgHeight = image.GetHeight();

            int addXY = (maxXY - imgWidth) / 2;
            int addZ = (maxZ - imgHeight) / 2;

            //int addSXY = (maxXY - size) / 2;

            int Xmin = addXY;
            if (Xmin < 0) Xmin = 0;
            int Xmax = imgWidth + addXY;
            if (Xmax > maxXY) Xmax = maxXY;

            int Zmin = addZ;
            if (Zmin < 0) Zmin = 0;
            int Zmax = imgHeight + addZ;
            if (Zmax > maxXY) Zmax = maxZ;

            int result = 0;
            for (int x = Xmin; x < Xmax; x++)
                //for (int y = Ymin; y < Ymax; y++)
                    for (int z = Zmin; z < Zmax; z++)
                    {
                        if (imgCopy[z, x].A != image.GetPixel(x - Xmin, z - Zmin).A)
                        {
                            result++;
                        }
                    }

            return result;//fix it
        }

        private static void FindBest(int z, int x)
        {
            int sumaDiff = 0;
            foreach (ImgStr imgStr in listTexture)
            {
                sumaDiff += GetDiffAlphaImg(imgStr.rotation, imgStr.texture2D);
            }
        }
        /*
.(0st)
123
804
765
↑
765,20 21 22
Y+ X+-

.(45st)
123
804
765
↗
18765
Y+ X-,20 21 12 00 22 

.(90st)
123
804
765
→
187
Y+- X-,00 10 20

.(135st)
123
804
765
↘
32187
Y- X-,00 01 10 02 20

.(180st)
123
804
765
↓
321
Y- X+-,02 01 00

.(225st)
123
804
765
↙
54321
Y- X+,02 01 12 00 22

.(270st)
123
804
765
←
543
Y+- X+,22 21 20

.(315st)
123
804
765
↖
76543,22 21 12 02 20
Y+ X+ 
*/
    }
}
