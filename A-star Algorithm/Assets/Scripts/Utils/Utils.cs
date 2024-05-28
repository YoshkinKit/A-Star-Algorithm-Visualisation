/* 
 * В цьому класі знаходяться корисні методи
 * які часто потрібні, або необхідні в декількох класах
 */



using System;
using UnityEngine;

public static class Utils
{
    // Цей метод створює текст на сцені
    public static TextMesh CreateWorldText(string text, Transform parent = null,
        Vector3 localPosition = default, int fontSize = 40, Color? color = null,
        TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left,
        int sortingOrder = 5000)
    {
        color ??= Color.white;

        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment,
            sortingOrder);
    }

    private static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize,
        Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
    
    // Цей метод дозволяє отримати позицію миші у глобальних координатах
    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vector = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vector.z = 0;
        return vector;
    }

    private static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        return worldCamera.ScreenToWorldPoint(screenPosition);
    }
    
    // Цей метод дозволяє отримати колір з шістнадцятирічного числа 
    public static Color GetColorFromString(string color)
    {
        float red = HexToDec01(color.Substring(0, 2));
        float green = HexToDec01(color.Substring(2, 2));
        float blue = HexToDec01(color.Substring(4, 2));
        float alpha = 1f;

        if (color.Length >= 8)
        {
            alpha = HexToDec01(color.Substring(6, 2));
        }

        return new Color(red, green, blue, alpha);
    }

    private static float HexToDec01(string hex)
    {
        return Convert.ToInt32(hex, 16) / 255f;
    }
}