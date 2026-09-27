"""Builds the embedded star lists for StarNameGenerator from three primary catalogues.

    python tools/StarLists/build_star_lists.py [--cache DIR]

Writes three files next to the library project (UTF-8, LF, one entry per line):

    Resources.stars.proper.stripped        IAU-approved proper names (WGSN catalogue, exopla.net)
    Resources.stars.designations.stripped  Bayer and Flamsteed names from the Yale Bright Star Catalogue
                                           (VizieR V/50), e.g. "Alpha Centauri", "61 Cygni", "Kappa¹ Sculptoris"
    Resources.stars.hipgaps.stripped       the numbers between 1 and the highest Hipparcos number (VizieR I/239)
                                           that are NOT in the catalogue; the first line is that highest number

Only the gaps are stored because the Hipparcos numbers are nearly contiguous: 2,198 gaps describe all 118,218
stars in 14 KB. Henry Draper numbers (HD 1 to HD 225300) are contiguous and need no file at all.
Downloads are cached in --cache (default: a folder in the system temp directory), so reruns are offline.
"""

import argparse
import html
import os
import re
import tempfile
import urllib.request

LIBRARY = os.path.join(os.path.dirname(__file__), "..", "..", "RandomNameGeneratorLibrary")

SOURCES = {
    "iau.html": "https://exopla.net/star-names/modern-iau-star-names/",
    "bsc5.tsv": "https://vizier.cds.unistra.fr/viz-bin/asu-tsv?-source=V/50/catalog&-out=HR&-out=Name&-out=HD&-out.max=unlimited",
    "hip.tsv": "https://vizier.cds.unistra.fr/viz-bin/asu-tsv?-source=I/239/hip_main&-out=HIP&-out.max=unlimited",
}

GREEK = {
    "Alp": "Alpha", "Bet": "Beta", "Gam": "Gamma", "Del": "Delta", "Eps": "Epsilon", "Zet": "Zeta",
    "Eta": "Eta", "The": "Theta", "Iot": "Iota", "Kap": "Kappa", "Lam": "Lambda", "Mu": "Mu", "Nu": "Nu",
    "Xi": "Xi", "Omi": "Omicron", "Pi": "Pi", "Rho": "Rho", "Sig": "Sigma", "Tau": "Tau", "Ups": "Upsilon",
    "Phi": "Phi", "Chi": "Chi", "Psi": "Psi", "Ome": "Omega",
}

# IAU constellation abbreviations to Latin genitives.
GENITIVE = {
    "And": "Andromedae", "Ant": "Antliae", "Aps": "Apodis", "Aqr": "Aquarii", "Aql": "Aquilae", "Ara": "Arae",
    "Ari": "Arietis", "Aur": "Aurigae", "Boo": "Boötis", "Cae": "Caeli", "Cam": "Camelopardalis",
    "Cnc": "Cancri", "CVn": "Canum Venaticorum", "CMa": "Canis Majoris", "CMi": "Canis Minoris",
    "Cap": "Capricorni", "Car": "Carinae", "Cas": "Cassiopeiae", "Cen": "Centauri", "Cep": "Cephei",
    "Cet": "Ceti", "Cha": "Chamaeleontis", "Cir": "Circini", "Col": "Columbae", "Com": "Comae Berenices",
    "CrA": "Coronae Australis", "CrB": "Coronae Borealis", "Crv": "Corvi", "Crt": "Crateris", "Cru": "Crucis",
    "Cyg": "Cygni", "Del": "Delphini", "Dor": "Doradus", "Dra": "Draconis", "Equ": "Equulei",
    "Eri": "Eridani", "For": "Fornacis", "Gem": "Geminorum", "Gru": "Gruis", "Her": "Herculis",
    "Hor": "Horologii", "Hya": "Hydrae", "Hyi": "Hydri", "Ind": "Indi", "Lac": "Lacertae", "Leo": "Leonis",
    "LMi": "Leonis Minoris", "Lep": "Leporis", "Lib": "Librae", "Lup": "Lupi", "Lyn": "Lyncis", "Lyr": "Lyrae",
    "Men": "Mensae", "Mic": "Microscopii", "Mon": "Monocerotis", "Mus": "Muscae", "Nor": "Normae",
    "Oct": "Octantis", "Oph": "Ophiuchi", "Ori": "Orionis", "Pav": "Pavonis", "Peg": "Pegasi", "Per": "Persei",
    "Phe": "Phoenicis", "Pic": "Pictoris", "Psc": "Piscium", "PsA": "Piscis Austrini", "Pup": "Puppis",
    "Pyx": "Pyxidis", "Ret": "Reticuli", "Sge": "Sagittae", "Sgr": "Sagittarii", "Sco": "Scorpii",
    "Scl": "Sculptoris", "Sct": "Scuti", "Ser": "Serpentis", "Sex": "Sextantis", "Tau": "Tauri",
    "Tel": "Telescopii", "Tri": "Trianguli", "TrA": "Trianguli Australis", "Tuc": "Tucanae",
    "UMa": "Ursae Majoris", "UMi": "Ursae Minoris", "Vel": "Velorum", "Vir": "Virginis", "Vol": "Volantis",
    "Vul": "Vulpeculae",
}

SUPERSCRIPT = str.maketrans("0123456789", "⁰¹²³⁴⁵⁶⁷⁸⁹")


def fetch(cache, name):
    path = os.path.join(cache, name)
    if not os.path.exists(path):
        request = urllib.request.Request(SOURCES[name], headers={"User-Agent": "DotNetRandomNameGenerator star list build"})
        with urllib.request.urlopen(request, timeout=300) as response, open(path, "wb") as out:
            out.write(response.read())
    with open(path, encoding="utf-8") as f:
        return f.read()


def proper_names(page):
    names = set()
    for row in re.findall(r"<tr[^>]*>(.*?)</tr>", page, re.S):
        cells = [html.unescape(re.sub(r"<[^>]+>", "", c)).strip() for c in re.findall(r"<td[^>]*>(.*?)</td>", row, re.S)]
        # The names table has 17 columns; the page's calendar widget also has <td> rows.
        if len(cells) >= 10 and cells[0] and cells[0].lower() != "proper names":
            names.add(re.sub(r"\s+", " ", cells[0]))
    return names


def designations(tsv):
    names = set()
    for line in tsv.splitlines():
        parts = line.split("\t")
        if line.startswith("#") or len(parts) < 3 or not parts[0].strip().isdigit():
            continue
        name, hd = parts[1], parts[2].strip()
        # Rows without an HD number are not stars (novae, 47 Tuc, other clusters).
        if not hd or len(name) != 10:
            continue
        flamsteed, bayer, component, constellation = name[0:3].strip(), name[3:6].strip(), name[6].strip(), name[7:10].strip()
        if constellation not in GENITIVE:
            continue
        genitive = GENITIVE[constellation]
        if bayer:
            names.add(GREEK[bayer] + component.translate(SUPERSCRIPT) + " " + genitive)
        elif flamsteed:
            names.add(flamsteed + " " + genitive)
    return names


def hipparcos_gaps(tsv):
    numbers = {int(line) for line in tsv.splitlines() if line.strip().isdigit()}
    highest = max(numbers)
    return [highest] + [n for n in range(1, highest + 1) if n not in numbers], len(numbers)


def write(name, lines):
    path = os.path.normpath(os.path.join(LIBRARY, name))
    with open(path, "w", encoding="utf-8", newline="\n") as f:
        f.write("\n".join(str(line) for line in lines) + "\n")
    print(f"{path}: {len(lines)} lines, {os.path.getsize(path):,} bytes")


def main():
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--cache", default=os.path.join(tempfile.gettempdir(), "star-list-sources"))
    args = parser.parse_args()
    os.makedirs(args.cache, exist_ok=True)

    proper = proper_names(fetch(args.cache, "iau.html"))
    designated = designations(fetch(args.cache, "bsc5.tsv")) - proper
    gaps, hip_count = hipparcos_gaps(fetch(args.cache, "hip.tsv"))

    write("Resources.stars.proper.stripped", sorted(proper))
    write("Resources.stars.designations.stripped", sorted(designated))
    write("Resources.stars.hipgaps.stripped", gaps)
    print(f"Hipparcos stars: {hip_count:,}")


if __name__ == "__main__":
    main()
