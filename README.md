# Csv2Collections for Unity

This package allows you to generate lists or dictionaries extracted from a CSV file.

<img src="https://user-images.githubusercontent.com/5264444/220840215-57dd8df8-b939-40c4-ae90-a9b11c014e9a.png">

## 日本語による説明 / Explanation in Japanese

[CSV の特定の列を抽出して、リストや辞書を生成できるツールを公開しました](https://blog.gigacreation.jp/entry/2023/02/22/175154)

## Usage

### Generate List from CSV

```cs
// 1. Prepare the CSV text you want to extract.
var csv = @"
name,model_number,release_date
Family Computer,HVC-001,1983-07-15
SUPER Famicom,SHVC-001,1990-11-21
NINTENDO64,NUS-001,1996-06-23
"

// 2. Create `CsvExtractRequest` .
var request = new CsvExtractRequest(csv, true, "", 1);

// 3. Pass the request to `CsvUtility.ExtractIntoList()` and you will get the extracted list!
List<string> list = CsvUtility.ExtractIntoList(request);
```

### Generate Dictionary from CSV

```cs
// 1. Prepare the CSV text you want to extract.
var csv = @"
name,model_number,release_date
Family Computer,HVC-001,1983-07-15
SUPER Famicom,SHVC-001,1990-11-21
NINTENDO64,NUS-001,1996-06-23
"

// 2. Create `CsvExtractRequest` .
var request = new CsvExtractRequest(csv, true, "", 1);
request.SetKeyColumnIndexes(0); // <- The indexes of the columns to extract as a key.

// 3. Pass the request to `CsvUtility.ExtractIntoDictionary()` and you will get the extracted dictionary!
Dictionary<string, string> dictionary = CsvUtility.ExtractIntoDictionary(request);
```

## CsvExtractRequest

`CsvExtractRequest` has the following members:

```cs
/// <summary>
/// The CSV text.
/// </summary>
public string Csv { get; }

/// <summary>
/// If true, the first line of the CSV file is treated as a header.
/// </summary>
public bool HasHeader { get; }

/// <summary>
/// The separator if multiple columns are specified.
/// </summary>
public string Separator { get; }

/// <summary>
/// The indexes of the columns to extract as a value.
/// </summary>
public int[] ValueColumnIndexes { get; }

/// <summary>
/// The indexes of the columns to extract as a key.
/// </summary>
public int[] KeyColumnIndexes { get; private set; }

public CsvExtractRequest(string csv, bool hasHeader = false, string separator = null, params int[] valueIndexes)
{
    Csv = csv;
    HasHeader = hasHeader;
    Separator = separator;
    ValueColumnIndexes = valueIndexes;
}

public void SetKeyColumnIndexes(params int[] indexes)
{
    KeyColumnIndexes = indexes;
}
```

## Installation

### Package Manager

- `https://github.com/gigacreation/Csv2CollectionsForUnity.git?path=Assets/Csv2Collections`

### Manual

- Copy `Assets/Csv2Collections/` to your project.
