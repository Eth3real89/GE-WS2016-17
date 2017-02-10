/////////////////////////////////////////////////////////////////////////////////////////////////////////
//////  EnviroTerrainSeasons - Switches Terrain Textures and grass color according current seasons //////
/////////////////////////////////////////////////////////////////////////////////////////////////////////
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Enviro/Seasons for Terrains")]
public class EnviroTerrainSeasons : MonoBehaviour {
	
	public Terrain terrain;
	public bool ChangeGrass = true;
	public bool ChangeGrassDensity = true;
	public bool ChangeTextures = true;

	//The Id of terrain Texture to Change
	public List<int> textureChangeIds = new List<int>();

	public Texture2D SpringTexture;
	public Texture2D SpringNormal;
	public Texture2D SummerTexture;
	public Texture2D SummerNormal;
	public Texture2D AutumnTexture;
	public Texture2D AutumnNormal;
	public Texture2D WinterTexture;
	public Texture2D WinterNormal;

	public Color SpringGrassColor;
	public Color SummerGrassColor;
	public Color AutumnGrassColor;
	public Color WinterGrassColor;

	public float SpringGrassDensity = 0.7f;
	public float SummerGrassDensity = 1f;
	public float AutumnGrassDensity = 1f;
	public float WinterGrassDensity = 0f;


	public Vector2 tiling = new Vector2(10f,10f);
	
	SplatPrototype[] textureInSplats  = new SplatPrototype[1];
	SplatPrototype[] texturesIn;   


	// Use this for initialization
	void Start () 
	{
		if (terrain == null)
			terrain = GetComponent<Terrain> ();
		texturesIn = terrain.terrainData.splatPrototypes;
		UpdateSeason ();

		EnviroMgr.instance.OnSeasonChanged += (SeasonVariables.Seasons season) =>
		{
			UpdateSeason ();
		};

	}


	// Check for correct Setup
	void OnEnable ()
	{
		if (ChangeTextures)
		{
			if(SpringTexture == null)
			{
			Debug.LogError("Please assign a spring texture in Inspector!");
			this.enabled = false;
			}
			if(SummerTexture == null)
			{
				Debug.LogError("Please assign a summer texture in Inspector!");
				this.enabled = false;
			}
			if(AutumnTexture == null)
			{
				Debug.LogError("Please assign a autumn texture in Inspector!");
				this.enabled = false;
			}
			if(WinterTexture == null)
			{
				Debug.LogError("Please assign a winter texture in Inspector!");
				this.enabled = false;
			}
			if (textureChangeIds.Count < 1)
			  {
				Debug.LogError("Please configure Texture ChangeSlot IDs!");
				this.enabled = false;
			 }
		}
	}

	void ChangeGrassColor (Color ChangeToColor)
	{
		terrain.terrainData.wavingGrassTint = ChangeToColor;
	}

	
	void ChangeTexture(Texture2D inTexture, int id)
	{        
		textureInSplats = texturesIn;
		textureInSplats[id].texture = inTexture; // texture here
		textureInSplats[id].tileSize= tiling; //tiling size
		terrain.terrainData.splatPrototypes = textureInSplats;
		terrain.Flush();
	}

	void ChangeTexture(Texture2D inTexture,Texture2D inNormal, int id)
	{        
		textureInSplats = texturesIn;
		textureInSplats[id].texture = inTexture; // texture here
		textureInSplats[id].normalMap = inNormal; // texture here
		textureInSplats[id].tileSize= tiling; //tiling size
		terrain.terrainData.splatPrototypes = textureInSplats;
		terrain.Flush();
	}


	void UpdateSeason ()
	{
		switch (EnviroMgr.instance.seasons.currentSeasons)
		{
		case SeasonVariables.Seasons.Spring:
			for (int i = 0 ; i < textureChangeIds.Count;i++)
			{
				if (ChangeTextures)
				{
					ChangeTexture(SpringTexture,textureChangeIds[i]);
					if (SpringNormal != null)
						ChangeTexture(SpringTexture,SpringNormal,textureChangeIds[i]);
				}
			}
			if(ChangeGrass)
				ChangeGrassColor(SpringGrassColor);
			if(ChangeGrassDensity)
				terrain.detailObjectDensity = SpringGrassDensity;
			break;
			
		case SeasonVariables.Seasons.Summer:
			for (int i = 0 ; i < textureChangeIds.Count;i++)
			{
				if (ChangeTextures)
				{
					ChangeTexture(SummerTexture,textureChangeIds[i]);
					if (SummerNormal != null)
						ChangeTexture(SummerTexture,SummerNormal,textureChangeIds[i]);
				}
			}
			if(ChangeGrass)
				ChangeGrassColor(SummerGrassColor);
			if(ChangeGrassDensity)
				terrain.detailObjectDensity = SummerGrassDensity;
			break;
			
		case SeasonVariables.Seasons.Autumn:
			for (int i = 0 ; i < textureChangeIds.Count;i++)
			{
				if (ChangeTextures)
				{
					ChangeTexture(AutumnTexture,textureChangeIds[i]);
					if (AutumnNormal != null)
						ChangeTexture(AutumnTexture,AutumnNormal,textureChangeIds[i]);
				}
			}
			if(ChangeGrass)
				ChangeGrassColor(AutumnGrassColor);
			if(ChangeGrassDensity)
				terrain.detailObjectDensity = AutumnGrassDensity;
			break;
			
		case SeasonVariables.Seasons.Winter:
			for (int i = 0 ; i < textureChangeIds.Count;i++)
			{
				if (ChangeTextures)
				{
					ChangeTexture(WinterTexture,textureChangeIds[i]);
					if (WinterNormal != null)
						ChangeTexture(WinterTexture,WinterNormal,textureChangeIds[i]);
				}
			}
			if(ChangeGrass)
				ChangeGrassColor(WinterGrassColor);

			if(ChangeGrassDensity)
				terrain.detailObjectDensity = WinterGrassDensity;
			break;
		}

	}
}
