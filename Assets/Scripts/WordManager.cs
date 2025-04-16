using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WordManager : MonoBehaviour
{
	public GameObject wordPrefab;
	public int wordsToSpawn = 20;
	public string[] wordList;
	public Vector2 spacing = new Vector2(0.1f, 0.1f); // Espacio mínimo entre palabras

	private List<Rect> occupiedRects = new List<Rect>();
	private int[] angles = { 0, -90 };

	void Start()
	{
		SpawnWords();
	}

	void SpawnWords()
	{
		for (int i = 0; i < wordsToSpawn; i++)
		{
			// Crear palabra y configurar
			GameObject wordObj = Instantiate(wordPrefab, Vector3.zero, Quaternion.identity, transform);
			TextMeshProUGUI text = wordObj.GetComponentInChildren<TextMeshProUGUI>();
			text.text = GenerateRandomWord();

			// Rotación y tamaño efectivo
			float rotationZ = angles[Random.Range(0, angles.Length)];
			wordObj.transform.rotation = Quaternion.Euler(0, 0, rotationZ);
			Vector2 size = (rotationZ % 180 == 0) ? new Vector2(2f, 1f) : new Vector2(1f, 2f);

			// Buscar posición válida (ajustada para pivot 0.5, 0.5)
			Vector2 position = FindValidPosition(size);
			wordObj.transform.localPosition = position;

			// Calcular Rect considerando el pivot central
			Rect newRect = new Rect(
				position.x - (size.x / 2f),
				position.y - (size.y / 2f),
				size.x,
				size.y
			);
			occupiedRects.Add(newRect);

			DebugDrawRect(newRect, Color.green);
		}
	}

	Vector2 FindValidPosition(Vector2 size)
	{
		if (occupiedRects.Count == 0) return Vector2.zero;

		// Buscar en todas las cajas existentes
		foreach (Rect existing in occupiedRects)
		{
			// Derecha: existing.xMax + size.x/2 (centro del nuevo objeto)
			Vector2 rightPos = new Vector2(existing.xMax + size.x / 2, existing.center.y);
			if (IsValidPosition(rightPos, size)) return rightPos;

			// Arriba: existing.yMax + size.y/2 (centro del nuevo objeto)
			Vector2 topPos = new Vector2(existing.center.x, existing.yMax + size.y / 2);
			if (IsValidPosition(topPos, size)) return topPos;
		}

		// Fallback: Nueva fila usando el Y máximo
		float maxY = GetMaxY() + size.y;
		return new Vector2(0, maxY);
	}
	bool IsValidPosition(Vector2 centerPosition, Vector2 size)
	{
		// Convertir posición central a esquina inferior-izquierda
		Rect testRect = new Rect(
			centerPosition.x - (size.x / 2f),
			centerPosition.y - (size.y / 2f),
			size.x,
			size.y
		);

		foreach (Rect existing in occupiedRects)
		{
			if (existing.Overlaps(testRect)) return false;
		}

		return true;
	}

	float GetMaxY()
	{
		float maxY = 0;
		foreach (Rect rect in occupiedRects)
		{
			if (rect.yMax > maxY) maxY = rect.yMax;
		}
		return maxY;
	}

	Vector2 FindOptimalPosition(Vector2 size)
	{
		// Primera palabra: posición inicial
		if (occupiedRects.Count == 0) return Vector2.zero;

		// Buscar en todas las posiciones posibles (derecha, arriba, y espacios vacíos)
		foreach (Rect existing in occupiedRects)
		{
			// Posición derecha (xMax + spacing)
			Vector2 rightPos = new Vector2(existing.xMax + spacing.x, existing.y);
			if (IsValidPosition(rightPos, size)) return rightPos;

			// Posición arriba (yMax + spacing)
			Vector2 topPos = new Vector2(existing.x, existing.yMax + spacing.y);
			if (IsValidPosition(topPos, size)) return topPos;
		}

		// Si no hay espacio, buscar en filas anteriores (evitar dejar huecos)
		float currentY = 0;
		while (true)
		{
			Vector2 candidatePos = new Vector2(0, currentY);
			if (IsValidPosition(candidatePos, size)) return candidatePos;
			currentY += size.y + spacing.y;
		}
	}

	void DebugDrawRect(Rect rect, Color color)
	{
		Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y), color);
		Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y), new Vector3(rect.x + rect.width, rect.y + rect.height), color);
		Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x, rect.y + rect.height), color);
		Debug.DrawLine(new Vector3(rect.x, rect.y + rect.height), new Vector3(rect.x, rect.y), color);
	}

	string GenerateRandomWord()
	{
		if (wordList.Length > 0)
			return wordList[Random.Range(0, wordList.Length)];

		// Generar palabra aleatoria (backup)
		string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		int length = Random.Range(4, 10);
		return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Range(0, s.Length)]).ToArray());
	}
}