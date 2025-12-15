using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using rnd = UnityEngine.Random;
using System.Text.RegularExpressions;

public class Heraldry : MonoBehaviour
{
	const bool T = true;
	const bool F = false;

	public KMBombInfo bomb;
	public KMAudio Audio;

	//Logging
	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;

	int animating = 0;
	const int CrestsToGenerate = 48;
	int currentCrest = 0;
	Crest[] crests = new Crest[CrestsToGenerate];
	List<int> order;
	//List<int> familyNamesOrder;
	int[] crestCount = Enumerable.Repeat(3, 16).ToArray();

	int grid;

	List<int>[] validDivitions = new List<int>[4];
	List<int>[] validTinctures = new List<int>[4];
	List<int>[] validCharges = new List<int>[4];

	bool unicorn = false;

	public GameObject axis;
	public KMSelectable[] pageTurners;
	public KMSelectable[] crestSelectors;
	public GameObject[] pages;
	public GameObject[] crestObjects;
	public Material[] materials;
	public TextMesh[] pageMeshes;

	bool[][][] grids = new bool[][][]
	{
		new bool [][] {
			new bool [] { F, T, F, F, F, F, F, T },
			new bool [] { F, F, F, F, F, F, T, F },
			new bool [] { T, F, F, F, F, F, F, T },
			new bool [] { F, F, F, F, T, F, F, F },
			new bool [] { F, F, T, F, F, T, F, F },
			new bool [] { F, F, F, F, F, T, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, T, T, F, F, F, F, T },
		},
		new bool [][] {
			new bool [] { F, T, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, T, F, T },
			new bool [] { F, F, T, F, T, F, F, F },
			new bool [] { F, T, F, F, F, T, F, F },
			new bool [] { F, F, F, T, F, F, F, F },
			new bool [] { F, F, F, F, F, F, F, T },
			new bool [] { F, T, F, F, F, F, F, T },
			new bool [] { T, F, F, F, F, F, F, F },
		},
		new bool [][] {
			new bool [] { F, F, T, F, F, F, T, F },
			new bool [] { T, T, F, F, F, F, F, F },
			new bool [] { F, F, F, T, T, T, F, F },
			new bool [] { F, F, F, F, F, F, T, F },
			new bool [] { F, F, F, F, T, F, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { T, F, F, T, F, F, F, T },
			new bool [] { F, F, F, F, F, F, F, F },
		},
		new bool [][] {
			new bool [] { F, F, F, T, F, F, T, F },
			new bool [] { T, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, F, T },
			new bool [] { F, F, F, T, F, F, F, F },
			new bool [] { F, T, T, T, T, F, F, F },
			new bool [] { T, F, T, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, T, F },
		},
		new bool [][] {
			new bool [] { F, T, F, F, F, T, T, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, T, F, F, F },
			new bool [] { T, F, T, T, T, F, F, F },
			new bool [] { F, F, F, F, T, F, T, T },
			new bool [] { T, F, F, F, F, F, F, F },
		},
		new bool [][] {
			new bool [] { F, F, F, F, F, F, T, F },
			new bool [] { T, F, T, F, F, F, F, F },
			new bool [] { F, F, F, T, F, F, T, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { T, F, F, T, F, T, F, F },
			new bool [] { F, F, F, F, F, T, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { T, F, F, F, T, F, T, F },
		},
		new bool [][] {
			new bool [] { F, F, F, F, T, F, F, F },
			new bool [] { T, F, F, F, F, F, F, T },
			new bool [] { F, T, T, F, F, F, F, F },
			new bool [] { F, F, T, F, T, F, F, F },
			new bool [] { F, F, T, F, F, F, F, T },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, T, F, T, F, F, F, T },
		},
		new bool [][] {
			new bool [] { F, T, T, F, F, F, F, T },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, T, T, F, F },
			new bool [] { T, F, F, T, F, T, F, F },
			new bool [] { F, F, F, F, F, F, T, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, T, F, F, F, F, T },
			new bool [] { F, T, F, F, F, F, F, F },
		},
		new bool [][] {
			new bool [] { T, F, F, F, F, F, T, F },
			new bool [] { F, F, F, F, F, F, T, F },
			new bool [] { F, F, T, F, T, F, F, F },
			new bool [] { F, F, F, F, T, T, F, F },
			new bool [] { F, T, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, T, F },
			new bool [] { F, T, F, F, F, F, F, T },
			new bool [] { F, T, F, F, F, F, F, F },
		},
		new bool [][] {
			new bool [] { T, F, F, F, F, F, F, T },
			new bool [] { F, F, F, F, F, T, F, F },
			new bool [] { F, F, F, T, F, F, F, F },
			new bool [] { F, F, F, F, F, F, F, T },
			new bool [] { T, F, F, F, T, T, F, F },
			new bool [] { F, F, T, F, F, F, F, F },
			new bool [] { T, F, F, F, F, T, F, F },
			new bool [] { F, F, F, F, F, F, T, F },
		},
		new bool [][] {
			new bool [] { F, F, F, F, T, F, T, F },
			new bool [] { T, T, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, T, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, T, T, F, T, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, T, F, F, T, F },
			new bool [] { T, F, F, F, F, F, T, F },
		},
		new bool [][] {
			new bool [] { T, F, F, F, F, F, T, F },
			new bool [] { F, F, T, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, T, F, F, F, T, F },
			new bool [] { F, T, F, T, F, F, F, F },
			new bool [] { F, F, F, F, T, T, F, F },
			new bool [] { T, F, F, F, F, F, F, T },
			new bool [] { F, F, F, F, T, F, F, F },
		},
		new bool [][] {
			new bool [] { F, T, F, F, F, F, F, F },
			new bool [] { F, F, F, F, T, F, F, T },
			new bool [] { T, F, F, T, F, T, F, F },
			new bool [] { F, F, T, F, F, F, F, F },
			new bool [] { F, F, F, F, T, F, F, T },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, F, T },
			new bool [] { T, F, T, F, F, F, F, F },
		},
		new bool [][] {
			new bool [] { F, F, F, F, F, F, F, T },
			new bool [] { T, F, F, T, F, F, F, F },
			new bool [] { F, F, T, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, T, T, F, F },
			new bool [] { F, T, F, F, T, F, F, T },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { T, F, F, T, F, F, F, T },
		},
		new bool [][] {
			new bool [] { F, T, F, F, F, T, T, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, T, F, F, T },
			new bool [] { F, F, F, F, T, T, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { T, F, F, F, F, T, F, F },
			new bool [] { F, F, F, F, T, F, F, T },
			new bool [] { T, F, F, F, F, F, F, F },
		},
		new bool [][] {
			new bool [] { F, T, F, T, F, F, F, T },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, T, F },
			new bool [] { F, T, F, F, F, F, F, F },
			new bool [] { F, F, T, T, F, F, F, F },
			new bool [] { F, F, T, T, F, F, F, F },
			new bool [] { T, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, T, T, F },
		},
	};

	int[][] validityTable = new int[][] {
		new int[] { Crest.EAGLE, Crest.DRAGON, Crest.LION, Crest.HOUND, Crest.BEAR, Crest.STAG, Crest.GULES, Crest.VERT },
		new int[] { Crest.HORSE, Crest.DOLPHIN, Crest.SERPENT, Crest.OX, Crest.BOAR, Crest.GRIFFIN, Crest.SEAHORSE, Crest.PURPURE },
		new int[] { Crest.GREEK, Crest.MOLINE, Crest.PLAIN, Crest.QUARTERLY, Crest.PILE, Crest.PARTYPERCHEVRON, Crest.PATONCE, Crest.FLORY },
		new int[] { Crest.POMMEE, Crest.CROSSLET, Crest.PARTYPERPALE, Crest.PARTYPERFESS, Crest.PARTYPERBEND, Crest.PARTYPERSALTIRE, Crest.POTENT, Crest.SALTIRE },
		new int[] { Crest.VOIDED, Crest.FOURCHEE, Crest.PALL, Crest.FESS, Crest.BEND, Crest.SALTIRE_DIV, Crest.PATTEE, Crest.MALTESE },
		new int[] { Crest.BOTTONY, Crest.MASCLE, Crest.CROSS, Crest.CHIEF, Crest.PALE, Crest.CHEVRON, Crest.FLEUR_DE_LIS, Crest.CROWN },
		new int[] { Crest.CELESTE, Crest.LYRE, Crest.SHELL, Crest.SUN, Crest.MOON, Crest.TOWER, Crest.KEYS, Crest.ROUNDEL },
		new int[] { Crest.AZURE, Crest.SABLE, Crest.SWORDS, Crest.FLOWER, Crest.LEAF, Crest.HAND, Crest.ANNULET, Crest.MULLET },
	};

	private readonly static Dictionary<string, int> crestDivisions = new Dictionary<string, int>
	{
		{ "Crest", -1 },

		{ "Bend", 3 },
		{ "Chevron", 3 },
		{ "Chief", 2 },
		{ "Cross", 5 },
		{ "Fess", 3 },
		{ "Pale", 3 },
		{ "Pall", 4 },
		{ "PartyPerBend", 2 },
		{ "PartyPerChevron", 2 },
		{ "PartyPerFess", 2 },
		{ "PartyPerPale", 2 },
		{ "PartyPerSaltire", 4 },
		{ "Pile", 3 },
		{ "Plain", 1 },
		{ "Quarterly", 4 },
		{ "Saltire", 5 },
	}, crestCharges = new Dictionary<string, int>
	{
		{ "Crest", -1 },

		{ "Bend", 3 },
		{ "Chevron", 3 },
		{ "Chief", 3 },
		{ "Cross", 2 },
		{ "Fess", 3 },
		{ "Pale", 3 },
		{ "Pall", 3 },
		{ "PartyPerBend", 1 },
		{ "PartyPerChevron", 2 },
		{ "PartyPerFess", 2 },
		{ "PartyPerPale", 1 },
		{ "PartyPerSaltire", 2 },
		{ "Pile", 1 },
		{ "Plain", 3 },
		{ "Quarterly", 2 },
		{ "Saltire", 4 },
	};


	void Awake()
	{
		moduleId = moduleIdCounter++;

		pageTurners[0].OnInteract += delegate () { PrevPage(); return false; };
		pageTurners[1].OnInteract += delegate () { NextPage(); return false; };
		crestSelectors[0].OnInteract += delegate () { SelectCrest(0); return false; };
		crestSelectors[1].OnInteract += delegate () { SelectCrest(1); return false; };
	}

	void PrevPage()
	{
		if(currentCrest <= 0 || animating > 0)
			return;

		StartCoroutine(PageTurnAnim(pages[(currentCrest / 2) - 1], -1));

		currentCrest -= 2;
	}

	void NextPage()
	{
		if(currentCrest >= CrestsToGenerate - 2 || animating < 0)
			return;

		StartCoroutine(PageTurnAnim(pages[currentCrest / 2], 1));

		currentCrest += 2;
	}

	void SelectCrest(int dif)
	{
		if(animating != 0 || moduleSolved)
			return;

		int selected = currentCrest + dif;

		int sol;
		var solveCount = bomb.GetSolvedModuleNames().Count();
		if (unicorn) sol = 1;
		else if(solveCount % 4 == 0) sol = 2;
		else if(solveCount % 4 == 1) sol = 3;
		else if(solveCount % 4 == 2) sol = 4;
		else sol = 5;

		if(selected == order[sol])
		{
			Debug.LogFormat("[Heraldry #{0}] Correctly selected Crest #{1} at {2} solve{3}. Module solved.", moduleId, order[sol] + 1, solveCount, solveCount == 1 ? "" : "s");
			moduleSolved = true;
			Audio.PlaySoundAtTransform("trumpet", transform);
            GetComponent<KMBombModule>().HandlePass();
		}
		else
		{
			Debug.LogFormat("[Heraldry #{0}] Strike! Selected Crest #{1} when Crest #{2} was expected at {3} solve{4}.", moduleId, selected + 1, order[sol] + 1, solveCount, solveCount == 1 ? "" : "s");
            GetComponent<KMBombModule>().HandleStrike();
		}

	}

	void Start () 
	{
		GenerateCrests();
		GetComponent<KMBombModule>().OnActivate += delegate {
            for (var x = 0; x < pageMeshes.Length; x++)
				pageMeshes[x].text = TwitchPlaysActive ? (currentCrest + 1 + x).ToString() : "";
		};
	}

	void GenerateCrests()
	{
		order = Enumerable.Range(0, CrestsToGenerate).OrderBy(x => rnd.Range(0, 10000)).ToList();

		int royal = rnd.Range(0, 16);
		crestCount[royal]--;
		crests[order[0]] = Crest.Build(royal, new int [] { Crest.OR }.ToList(), new int[] { Crest.LION }.ToList(), false, false, false, true);

		int unicorn;
		do unicorn = rnd.Range(0, 16); while (unicorn == royal);
		crestCount[unicorn]--;
		crests[order[1]] = Crest.Build(unicorn, new int [] { Crest.ARGENT }.ToList(), new int[] { Crest.UNICORN }.ToList(), false, false, true, false);

		grid = crests[order[0]].GetScore(bomb) % 16;
		if(grid == 0)
			grid = 16;
		grid--;

		CalcValid();

		validDivitions[0] = validDivitions[0].OrderBy(x => rnd.Range(0, 10000)).ToList();
		validDivitions[1] = validDivitions[1].OrderBy(x => rnd.Range(0, 10000)).ToList();
		validDivitions[2] = validDivitions[2].OrderBy(x => rnd.Range(0, 10000)).ToList();
		validDivitions[3] = validDivitions[3].OrderBy(x => rnd.Range(0, 10000)).ToList();

		crestCount[validDivitions[0][0]]--;
		crests[order[2]] = Crest.Build(validDivitions[0][0], validTinctures[0], validCharges[0], true);
		crestCount[validDivitions[1][0]]--;
		crests[order[3]] = Crest.Build(validDivitions[1][0], validTinctures[1], validCharges[1], true);
		crestCount[validDivitions[2][0]]--;
		crests[order[4]] = Crest.Build(validDivitions[2][0], validTinctures[2], validCharges[2], true);
		crestCount[validDivitions[3][0]]--;
		crests[order[5]] = Crest.Build(validDivitions[3][0], validTinctures[3], validCharges[3], true);

		validDivitions[0] = validDivitions[0].OrderBy(x => rnd.Range(0, 10000)).ToList();
		validDivitions[1] = validDivitions[1].OrderBy(x => rnd.Range(0, 10000)).ToList();
		validDivitions[2] = validDivitions[2].OrderBy(x => rnd.Range(0, 10000)).ToList();
		validDivitions[3] = validDivitions[3].OrderBy(x => rnd.Range(0, 10000)).ToList();

		crestCount[validDivitions[0][0]]--;
		crests[order[6]] = Crest.Build(validDivitions[0][0], validTinctures[0], validCharges[0], false, true);
		crestCount[validDivitions[1][0]]--;
		crests[order[7]] = Crest.Build(validDivitions[1][0], validTinctures[1], validCharges[1], false, true);
		crestCount[validDivitions[2][0]]--;
		crests[order[8]] = Crest.Build(validDivitions[2][0], validTinctures[2], validCharges[2], false, true);
		crestCount[validDivitions[3][0]]--;
		crests[order[9]] = Crest.Build(validDivitions[3][0], validTinctures[3], validCharges[3], false, true);

		int cnt = 10;

		for(int i = 0; i < validDivitions.Length && cnt < CrestsToGenerate; i++)
			for(int j = 0; j < validDivitions[i].Count && cnt < CrestsToGenerate; j++)
				while(crestCount[validDivitions[i][j]] > 0 && cnt < CrestsToGenerate)
				{
					crestCount[validDivitions[i][j]]--;
					crests[order[cnt]] = Crest.Build(validDivitions[i][j], validTinctures[i], validCharges[i], false);
					cnt++;
				}
		
		for(int i = 1; i < order.Count; i++)
			if(crests.Count(x => x.familyName == crests[order[i]].familyName) != 1)
			{
				string name;
				do name = Crest.familyNames.OrderBy(x => rnd.Range(0, 100000)).ToList()[0]; while (crests.Count(x => x.familyName == name) != 0);
				crests[order[i]].familyName = name;
			}

		Log();

		for(int i = 0; i < order.Count; i++)
        	crests[order[i]].Paint(crestObjects[order[i]], materials);
	}

	void CalcValid()
	{
		for(int i = 0; i < validDivitions.Length; i++)
		{
			validDivitions[i] = new List<int>();
			validTinctures[i] = new List<int>();
			validCharges[i] = new List<int>();

			validTinctures[i].Add(Crest.OR);
			validTinctures[i].Add(Crest.ARGENT);
			validTinctures[i].Add(Crest.SANGUINE);
			validTinctures[i].Add(Crest.MURREY);
			validTinctures[i].Add(Crest.TENNE);

			for(int j = 0; j < grids[grid].Length; j++)
				for(int k = 0; k < grids[grid][j].Length; k++)
					if(grids[grid][j][k])
						if(j >= 2 && j <= 5 && k >= 2 && k <= 5)
							validDivitions[i].Add(validityTable[j][k]);
						else if((j == 0 && k >= 6) || (j == 1 && k == 7) || (j == 7 && k <= 1) || (j == 6 && k == 0))
							validTinctures[i].Add(validityTable[j][k]);
						else
							validCharges[i].Add(validityTable[j][k]);

			RotateGrid(grid);
		}
	}

	void RotateGrid(int g)
	{
		bool[][] temp = new bool [][] {
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
			new bool [] { F, F, F, F, F, F, F, F },
		};

		for(int i = 0; i < grids[g].Length; i++)
			for(int j = 0; j < grids[g][i].Length; j++)
				temp[j][grids[g].Length - 1 - i] = grids[g][i][j];

		grids[g] = temp;
	}

	void Log()
	{
		Debug.LogFormat("[Heraldry #{0}] -------------- Crests --------------", moduleId);

		for(int i = 0; i < crests.Length; i++)
			Debug.LogFormat("[Heraldry #{0}] (Crest #{1}) {2}.", moduleId, i + 1, crests[i].Description());

		Debug.LogFormat("[Heraldry #{0}] -------------- Solving --------------", moduleId);

		var royalCrest = crests[order[0]];

		// Crest Scoring
		var crestName = royalCrest.GetType().Name;
		var crestScore = 0;
		Debug.LogFormat("[Heraldry #{0}] Crest Type: {1}", moduleId, crestName);
		Debug.LogFormat("[Heraldry #{0}] Field Divisions in Crest Type: {1}", moduleId, crestDivisions[crestName]);
		crestScore += crestDivisions[crestName] * 2;
		Debug.LogFormat("[Heraldry #{0}] Crest type {1} \"Party\" in its name.", moduleId, crestName.Contains("Party") ? "has" : "does not have");
		if (crestName.Contains("Party"))
			crestScore--;
		var notSymmetricCrestNames = new[] { "PartyPerBend", "PartyPerPale", "Quarterly", "Bend" };
		Debug.LogFormat("[Heraldry #{0}] Crest type is {1}symmetrical across the vertical axis.", moduleId, notSymmetricCrestNames.Contains(crestName) ? "not " : "");
		if (!notSymmetricCrestNames.Contains(crestName))
			crestScore += 3;
		Debug.LogFormat("[Heraldry #{0}] Division of Field Score: {1}", moduleId, crestScore);
		// Family Name Scoring
		var familyName = royalCrest.familyName;
		var familyNameScore = 0;
		Debug.LogFormat("[Heraldry #{0}] Family Name: {1}", moduleId, familyName);
		Debug.LogFormat("[Heraldry #{0}] Number of letters in family name: {1}", moduleId, familyName.Count(a => !char.IsWhiteSpace(a) && a != '\''));
		Debug.LogFormat("[Heraldry #{0}] Number of words in family name: {1}", moduleId, familyName.Count(a => char.IsWhiteSpace(a) || a == '\'') + 1);
		familyNameScore += familyName.Select(a => char.IsWhiteSpace(a) || a == '\'' ? -1 : 1).Sum() - 1;
		Debug.LogFormat("[Heraldry #{0}] Number of letters in serial number in family name: {1}", moduleId, bomb.GetSerialNumberLetters().Count(x => familyName.ToUpperInvariant().ToArray().Contains(x)));
		familyNameScore += 4 * bomb.GetSerialNumberLetters().Count(x => familyName.ToUpperInvariant().ToArray().Contains(x));
		Debug.LogFormat("[Heraldry #{0}] Family Name Score: {1}", moduleId, familyNameScore);
		// Charge Scoring
		var charges = royalCrest.GetCharges();
		Debug.LogFormat("[Heraldry #{0}] Number of Animal Charges: {1}", moduleId, crestCharges[crestName] * charges.Count(a => a >= Crest.LION && a <= Crest.UNICORN));
		Debug.LogFormat("[Heraldry #{0}] Number of Cross Charges: {1}", moduleId, crestCharges[crestName] * charges.Count(a => a >= Crest.GREEK && a <= Crest.BOTTONY));
		Debug.LogFormat("[Heraldry #{0}] Number of Misc Charges: {1}", moduleId, crestCharges[crestName] * charges.Count(a => a > Crest.BOTTONY));
		Debug.LogFormat("[Heraldry #{0}] Charge Score: {1}", moduleId, 0);
		// Tincture Scoring
		var tints = royalCrest.GetColors();
		var tintScore = 0;
		var GAVTints = new[] { Crest.GULES, Crest.AZURE, Crest.VERT };
		var PSBCTints = new[] { Crest.PURPURE, Crest.SABLE, Crest.CELESTE };
		var StainTints = new[] { Crest.MURREY, Crest.SANGUINE, Crest.TENNE };
		Debug.LogFormat("[Heraldry #{0}] Gules, Azure, or Vert is {1}present in the royal crest.", moduleId, GAVTints.Any(a => tints.Contains(a)) ? "" : "not ");
		if (GAVTints.Any(a => tints.Contains(a)))
			tintScore += 2;
		Debug.LogFormat("[Heraldry #{0}] Purpure, Sable, or Bleu-Celeste is {1}present in the royal crest.", moduleId, PSBCTints.Any(a => tints.Contains(a)) ? "" : "not ");
		if (PSBCTints.Any(a => tints.Contains(a)))
			tintScore--;
		Debug.LogFormat("[Heraldry #{0}] A stain is {1}present in the royal crest.", moduleId, StainTints.Any(a => tints.Contains(a)) ? "" : "not ");
		if (StainTints.Any(a => tints.Contains(a)))
			tintScore += 5;
		Debug.LogFormat("[Heraldry #{0}] Tincture Score: {1}", moduleId, tintScore);

		// Final Score
		Debug.LogFormat("[Heraldry #{0}] Royal Family Crest score: {1} (Grid {2}).", moduleId, royalCrest.GetScore(bomb), grid + 1);

		if (bomb.GetBatteryCount() == 2 && bomb.GetBatteryHolderCount() == 1 && bomb.IsIndicatorOn(Indicator.FRK) && !bomb.IsPortPresent(Port.Serial) && !bomb.IsPortPresent(Port.Parallel))
		{
			unicorn = true;
			Debug.LogFormat("[Heraldry #{0}] The Order of the Unicorn calls for you. Discard the Royal Family Crest score. Solution is Crest #{1}.", moduleId, order[1] + 1);
		}
		else
		{
			for(int i = 0; i < validDivitions.Length; i++)
			{
				Debug.LogFormat("[Heraldry #{0}] Valid Divisions of Field for 4x + {1} solves: {2}.", moduleId, i, validDivitions[i].Select(x => Crest.divisionNames[x]).Join(", "));
				Debug.LogFormat("[Heraldry #{0}] Valid Tinctures for 4x + {1} solves: {2}.", moduleId, i, validTinctures[i].Select(x => Crest.fieldNames[x]).Join(", "));
				Debug.LogFormat("[Heraldry #{0}] Valid Charges for 4x + {1} solves: {2}.", moduleId, i, validCharges[i].Select(x => Crest.chargeNames[x]).Join(", "));
				Debug.LogFormat("[Heraldry #{0}] Solution for 4x + {1} solves: Crest #{2}.", moduleId, i, order[i + 2] + 1);
			}
		}
		Debug.LogFormat("[Heraldry #{0}] -------------- User Interactions --------------", moduleId);
	}

	IEnumerator PageTurnAnim(GameObject page, int direction)
	{
		animating += direction;
		var newPage = currentCrest + 2 * direction + 1;


		Audio.PlaySoundAtTransform("pageTurn", transform);

		float heightDif = page.transform.localPosition.y - 0.0145f;
		pageMeshes[direction > 0 ? 1 : 0].text = TwitchPlaysActive ? (newPage + (direction > 0 ? 1 : 0)).ToString() : "";
		for (int i = 0; i < 20; i++)
		{
			page.transform.RotateAround(axis.transform.position, axis.transform.up, 9f * direction);
			yield return new WaitForSeconds(0.01f);
		}
		pageMeshes[direction > 0 ? 0 : 1].text = TwitchPlaysActive ? (newPage + (direction > 0 ? 0 : 1)).ToString() : "";
		page.transform.localPosition = new Vector3(page.transform.localPosition.x, 0.0168f - heightDif, page.transform.localPosition.z);
		animating -= direction;
	}

    //twitch plays handler, originally by eXish, modified by VFlyer
    private bool cmdIsValid(string cmd)
    {
        char[] valids = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
        if ((cmd.Length >= 1) && (cmd.Length <= 2))
        {
            foreach (char c in cmd)
            {
                if (!valids.Contains(c))
                {
                    return false;
                }
            }
            int temp;
            return int.TryParse(cmd, out temp) && temp >= 1 && temp <= CrestsToGenerate;
        }
        else
        {
            return false;
        }
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = "\"!{0} cycle\" [Quickly cycles through all the crests] | \"!{0} crest <#>\" [Goes to crest #, valid #'s are 1-48] | !{0} \"submit left/right\" [Submits the currently shown crest on the left or right]";
	private bool TwitchPlaysActive;
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*cycle\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
			var lastCrest = currentCrest;
            while (currentCrest != 0)
            {
                pageTurners[0].OnInteract();
				yield return "trywaitcancel 0.1 The crest cycling has been halted due to a request to cancel!";
			}
            while (currentCrest < CrestsToGenerate - 2)
            {
                pageTurners[1].OnInteract();
                yield return "trywaitcancel 1.0 The crest cycling has been halted due to a request to cancel!";
            }
            while (currentCrest != lastCrest)
            {
                pageTurners[0].OnInteract();
				yield return "trywaitcancel 0.1 Restoring to the original crest has been halted due to a request to cancel!";
			}
            yield break;
        }
        if (Regex.IsMatch(command, @"^\s*submit l(eft)?\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            crestSelectors[0].OnInteract();
            yield break;
        }
        if (Regex.IsMatch(command, @"^\s*submit r(ight)?\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            crestSelectors[1].OnInteract();
            yield break;
        }
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*crest\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length == 2)
            {
                if (cmdIsValid(parameters[1]))
                {
                    yield return null;
                    int temp = 0;
                    int.TryParse(parameters[1], out temp);
					if (temp % 2 == 0)
                    {
                        temp -= 1;
                    }
                    if ((currentCrest + 1) < temp)
                    {
						while ((currentCrest + 1) != temp)
						{
							pageTurners[1].OnInteract();
							yield return "trywaitcancel 0.1 Accessing the specified crest has been canceled!";
						}
                    }
                    else if ((currentCrest + 1) > temp)
                    {
                        while ((currentCrest + 1) != temp)
						{
							pageTurners[0].OnInteract();
							yield return "trywaitcancel 0.1 Accessing the specified crest has been canceled!";
						}
                    }
                    else
                    {
                        yield return "sendtochat I'm already showing this crest, {0}!";
                    }
                }
            }
            yield break;
        }
    }
}
