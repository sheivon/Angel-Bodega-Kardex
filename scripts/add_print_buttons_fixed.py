from pathlib import Path
root = Path('Pages')
count = 0
for f in root.rglob('*.cshtml'):
    text = f.read_text(encoding='utf-8')
    if 'onclick="printTable(' in text or "onclick='printTable(" in text:
        continue
    if '<table class="table table-striped datatable mb-0">' in text:
        new = text.replace(
            '<table class="table table-striped datatable mb-0">',
            '<div class="d-flex justify-content-end p-3"><button type="button" class="btn btn-outline-primary btn-sm" onclick="printTable(\'.datatable\', \'Imprimir datos\')">Imprimir</button></div>\n                <table class="table table-striped datatable mb-0">'
        )
    elif '<table id="tablaPeriodos" class="table table-striped table-bordered w-100 datatable">' in text:
        new = text.replace(
            '<table id="tablaPeriodos" class="table table-striped table-bordered w-100 datatable">',
            '<div class="d-flex justify-content-end p-3"><button type="button" class="btn btn-outline-primary btn-sm" onclick="printTable(\'#tablaPeriodos\', \'Imprimir periodos\')">Imprimir</button></div>\n                    <table id="tablaPeriodos" class="table table-striped table-bordered w-100 datatable">'
        )
    else:
        new = text
    if new != text:
        f.write_text(new, encoding='utf-8')
        count += 1
print(f'Updated {count} files')
