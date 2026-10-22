import { spawn } from 'node:child_process';
import { existsSync, readFileSync } from 'node:fs';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const raiz = resolve(dirname(fileURLToPath(import.meta.url)), '..');
const rutaEnv = resolve(raiz, '.env');

if (!existsSync(rutaEnv)) {
  console.error('Falta frontend/.env. Crea el archivo con API_URL y MAILPIT_URL.');
  process.exit(1);
}

const variables = leerEnv(readFileSync(rutaEnv, 'utf8'));

for (const clave of ['API_URL', 'MAILPIT_URL']) {
  if (!variables[clave]) {
    console.error(`Falta ${clave} en frontend/.env.`);
    process.exit(1);
  }
}

const comando = process.argv[2];

if (!comando) {
  console.error('Indica el comando de Angular, por ejemplo: serve o build.');
  process.exit(1);
}

const ng = resolve(raiz, 'node_modules/@angular/cli/bin/ng.js');
const defines = ['API_URL', 'MAILPIT_URL'].map(
  (clave) => `--define=${clave}=${JSON.stringify(variables[clave])}`,
);

const proceso = spawn(process.execPath, [ng, comando, ...process.argv.slice(3), ...defines], {
  cwd: raiz,
  stdio: 'inherit',
});

proceso.on('exit', (codigo) => process.exit(codigo ?? 1));

function leerEnv(texto) {
  const valores = {};

  for (const linea of texto.split(/\r?\n/)) {
    const limpia = linea.trim();
    if (!limpia || limpia.startsWith('#')) continue;

    const separador = limpia.indexOf('=');
    if (separador === -1) continue;

    const clave = limpia.slice(0, separador).trim();
    let valor = limpia.slice(separador + 1).trim();

    if (
      (valor.startsWith('"') && valor.endsWith('"')) ||
      (valor.startsWith("'") && valor.endsWith("'"))
    ) {
      valor = valor.slice(1, -1);
    }

    valores[clave] = valor;
  }

  return valores;
}
