using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CrownDrawer : MonoBehaviour
{
    void Start()
    {
        GetComponent<Image>().sprite = CreateCrown();
    }

    Sprite CreateCrown()
    {
        int size = 256;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);

        // เคลียร์ให้ใสก่อน
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                tex.SetPixel(x, y, Color.clear);

        Color gold = new Color(1f, 0.85f, 0f);
        Color darkGold = new Color(0.88f, 0.56f, 0f);

        // มงกุฎ = polygon 7 จุด
        Vector2[] crown = new Vector2[]
        {
            new Vector2(0.08f,  0.20f),  // ซ้ายล่าง
            new Vector2(0.08f,  0.65f),  // ซ้ายบน
            new Vector2(0.28f,  0.45f),  // หยักซ้าย
            new Vector2(0.50f,  0.85f),  // ยอดกลาง
            new Vector2(0.72f,  0.45f),  // หยักขวา
            new Vector2(0.92f,  0.65f),  // ขวาบน
            new Vector2(0.92f,  0.20f),  // ขวาล่าง
        };

        // แปลง 0-1 เป็น pixel
        Vector2[] pts = new Vector2[crown.Length];
        for (int i = 0; i < crown.Length; i++)
            pts[i] = new Vector2(crown[i].x * size, crown[i].y * size);

        // วาด polygon
        FillPolygon(tex, pts, gold);

        // วาดแถบล่าง
        FillRect(tex,
            Mathf.RoundToInt(0.08f * size),
            Mathf.RoundToInt(0.10f * size),
            Mathf.RoundToInt(0.84f * size),
            Mathf.RoundToInt(0.12f * size),
            darkGold);

        // วาดอัญมณี 3 เม็ด
        DrawCircle(tex, new Vector2(0.08f * size, 0.65f * size), 10, Color.white);
        DrawCircle(tex, new Vector2(0.50f * size, 0.85f * size), 12, Color.white);
        DrawCircle(tex, new Vector2(0.92f * size, 0.65f * size), 10, Color.white);

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    void FillPolygon(Texture2D tex, Vector2[] pts, Color color)
    {
        int w = tex.width, h = tex.height;
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                if (PointInPolygon(new Vector2(x, y), pts))
                    tex.SetPixel(x, y, color);
    }

    void FillRect(Texture2D tex, int x, int y, int w, int h, Color color)
    {
        for (int px = x; px < x + w; px++)
            for (int py = y; py < y + h; py++)
                tex.SetPixel(px, py, color);
    }

    void DrawCircle(Texture2D tex, Vector2 center, int radius, Color color)
    {
        for (int x = -radius; x <= radius; x++)
            for (int y = -radius; y <= radius; y++)
                if (x * x + y * y <= radius * radius)
                    tex.SetPixel(
                        Mathf.RoundToInt(center.x) + x,
                        Mathf.RoundToInt(center.y) + y,
                        color);
    }

    bool PointInPolygon(Vector2 p, Vector2[] poly)
    {
        bool inside = false;
        int j = poly.Length - 1;
        for (int i = 0; i < poly.Length; j = i++)
        {
            if ((poly[i].y > p.y) != (poly[j].y > p.y) &&
                p.x < (poly[j].x - poly[i].x) * (p.y - poly[i].y)
                    / (poly[j].y - poly[i].y) + poly[i].x)
                inside = !inside;
        }
        return inside;
    }
}