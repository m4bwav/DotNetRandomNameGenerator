# Census sources of the embedded lists

The person and place lists in `RandomNameGeneratorLibrary/Resources.*.stripped` are rebuilt from these US Census files (public domain) with this tool. Downloaded and checked on 2026-09-28; a rebuild from files with these hashes must give the embedded files byte for byte.

| File | URL | SHA-256 | Embedded list |
|---|---|---|---|
| places2k.zip (holds places2k.txt) | https://www2.census.gov/geo/docs/maps-data/data/gazetteer/places2k.zip | 520a8374094b57aab5a64ad77b3d9def9a21158b85aae4fca7642e20e093bbc9 | |
| places2k.txt | inside the zip | f3a49faf62cde4c89334e8b11bcfca64a9990a82f744056e8aac230bcd27e92e | Resources.places2k.txt.stripped (16,969 names, since 2.3.0) |
| dist.all.last | https://www2.census.gov/topics/genealogy/1990surnames/dist.all.last | b0e2b3743ccbad641ca48b344c24cdebcd1d9a1f76dc6dbf05986f2919f0b4e1 | Resources.dist.all.last.stripped (88,799) |
| dist.male.first | https://www2.census.gov/topics/genealogy/1990surnames/dist.male.first | 0a5078ef6effe3b483d15b0f7f95047662126c9bfb624ecd5e5b978fc0f2470b | Resources.dist.male.first.stripped (1,219) |
| dist.female.first | https://www2.census.gov/topics/genealogy/1990surnames/dist.female.first | bd2f310fc4e5d5e5ea122c9d4342c9821145823118eb20db1647f305ec77b358 | Resources.dist.female.first.stripped (4,275) |

```
dotnet run --project tools/CensusTools -c Release -- place places2k.txt RandomNameGeneratorLibrary/Resources.places2k.txt.stripped
dotnet run --project tools/CensusTools -c Release -- person dist.all.last RandomNameGeneratorLibrary/Resources.dist.all.last.stripped
```

History: until 2.2.0 the place list was the output of the 2014 in-library stripper, which cut a name at the first "town", "city", "CDP", "village", "municipality", "borough" or "(balance)" anywhere in it (Georgetown became "George"); 2.1.0 fixed accents and duplicates by hand. 2.3.0 is the first list rebuilt with this tool. The person lists already matched a rebuild exactly. Changing any list changes seeded output, so a rebuild is a release decision (see the golden tests).

The star lists come from `tools/StarLists/build_star_lists.py`.
