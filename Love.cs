using Godot;
using System;
using System.Collections.Generic;

public class Love : Node2D {
    [Export] float cycleTime = 1;
    [Export] int cellSize = 10;
    [Export] int cellWidth = 60;
    [Export] int cellHeight = 60;
    [Export] Color activeCell = new Color(1,1,1);
    [Export] Font textFont;
    Random rand;
    float timer = 0;

    int[,] X;
    int[,] Y;
    Color[,] colors;
    int s = 0;
    float[,] colW;
    float color = 0;
    int il1; int ir1; int jl1; int jr1;
    bool paused;

    public override void _Ready() {
        // Create containers
        X = new int[cellWidth+1, cellHeight+1];
        Y = new int[cellWidth+1, cellHeight+1];
        colW = new float[cellWidth+1, cellHeight+1];
        colors = new Color[cellWidth+1, cellHeight+1];
        // Clear all
        rand = new Random((int)DateTime.Now.Ticks);
        foreach (int clr1 in GD.Range(0, 61, 1)) {
            foreach (int clr2 in GD.Range(0,61,1)) {
                X[clr1, clr2] = 0;
                Y[clr1, clr2] = 0;
            }
        }
        // Set starting points.
        Y[32,31] = 1;
        Y[33,31] = 1;
        Y[31,32] = 1;
        Y[32,32] = 1;
        Y[32,33] = 1;
        Y[59,59] = 1;
        // Activate node.
        SetProcess(true);
    }


    public override void _Process(float delta) {
        //
        Update();
        // Pause - Run
        if (Input.IsActionJustPressed("ui_accept")) paused = !paused;
        // Time control
        if (Input.IsActionPressed("ui_up")) {
            cycleTime = Mathf.Clamp(cycleTime - delta, 0.05f, 2);
        } else if (Input.IsActionPressed("ui_down")) {
            cycleTime = Mathf.Clamp(cycleTime + delta, 0.05f, 2);
        }
        // Click Spawn
        if (Input.IsMouseButtonPressed(1)) {
            //
            Vector2 mousePos = GetLocalMousePosition();
            int cx = (int) (mousePos.x / cellSize);
            int cy = (int) (mousePos.y / cellSize);
            if (cx >= 0 && cy >= 0 && cx < cellWidth && cy < cellHeight) Y[cx,cy] = 1;
        }
        // Click Delete
        if (Input.IsMouseButtonPressed(2)) {
            //
            Vector2 mousePos = GetLocalMousePosition();
            int cx = (int) (mousePos.x / cellSize);
            int cy = (int) (mousePos.y / cellSize);
            if (cx >= 0 && cy >= 0 && cx < cellWidth && cy < cellHeight) Y[cx,cy] = 0;
        }
        if (!paused) timer += delta;
    }

    private void ExecuteCycle() {
        if (timer < cycleTime) return;
        timer -= cycleTime;
        foreach (int i in GD.Range(0, cellWidth, 1)) {
            foreach (int j in GD.Range(0, cellHeight, 1)) {
                il1 = Mathf.PosMod((i-1)+cellWidth, cellWidth);
                ir1 = Mathf.PosMod((i+1)+cellWidth, cellWidth);
                jl1 = Mathf.PosMod((j-1)+cellHeight, cellHeight);
                jr1 = Mathf.PosMod((j+1)+cellHeight, cellHeight);
                s = X[il1,jl1] + X[il1,j] + X[il1,jr1] + X[i,jl1] + X[i,jr1] + X[ir1,jl1] + X[ir1,j] + X[ir1,jr1];
                //
                if (s == 3) { // BORN
                    Y[i,j] = 1;
                } else if (s == 2) { // LIVE
                    Y[i,j] = X[i,j];
                } else { // DIE
                    Y[i,j] = 0;
                }
            }
        }
    }

    public override void _Draw() {
        float weight = timer / cycleTime;
        foreach (int i in GD.Range(0, cellWidth, 1)) {
            foreach (int j in GD.Range(0, cellHeight, 1)) {
                //if (X[i,j] != Y[i,j]) colW[i,j] = X[i,j];
                X[i,j] = Y[i,j];
                colW[i,j] = Mathf.Lerp(colW[i,j], X[i,j], weight);
                color = colW[i,j];
                colors[i,j] = new Color(activeCell.r * color, activeCell.g * color, activeCell.b * color);
                DrawRect(new Rect2(i*cellSize, j*cellSize, cellSize, cellSize), colors[i,j]);
            }
        }
        if (paused) DrawString(textFont, new Vector2(64,64), "PAUSED");
        ExecuteCycle();
    }
}
