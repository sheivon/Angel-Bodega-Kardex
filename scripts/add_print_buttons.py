from pathlib import Path
root = Path('Pages')
count = 0
for f in root.rglob('*.cshtml'):
    text = f.read_text(encoding='utf-8')
    new = text
    if 'onclick="printTable(' in text:
        continue
    new = new.replace(
        '<table class="table table-striped datatable mb-0">',
        '<div class="d-flex justify-content-end p-3"><button type="button" class="btn btn-outline-primary btn-sm" onclick="printTable(\'.datatable\', \'Imprimir datos\')">Imprimir</button></div>\n                <table class="table table-striped datatable mb-0">'
    )
    new = new.replace(
        '<table id="tablaPeriodos" class="table table-striped table-bordered w-100 datatable">',
        '<div class="d-flex justify-content-end p-3"><button type="button" class="btn btn-outline-primary btn-sm" onclick="printTable(\'#tablaPeriodos\', \'Imprimir periodos\')">Imprimir</button></div>\n                    <table id="tablaPeriodos" class="table table-striped table-bordered w-100 datatable">'
    )
    if new != text:
        f.write_text(new, encoding='utf-8')
        count += 1
print(f'Updated {count} files')
