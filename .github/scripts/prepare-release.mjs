import { execFileSync, spawnSync } from 'node:child_process';
import { appendFileSync, readFileSync, writeFileSync } from 'node:fs';
import path from 'node:path';

const git = (...args) => execFileSync('git', args, {
  encoding: 'utf8',
  stdio: ['ignore', 'pipe', 'pipe'],
}).trim();

try {
  const tag = process.env.RELEASE_TAG;
  if (!/^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)$/.test(tag ?? '')) {
    throw new Error('Release tag must be a version without a prefix, for example 0.4.1.');
  }

  const commit = git('rev-parse', '--verify', `refs/tags/${tag}^{commit}`);
  if (git('rev-parse', 'HEAD') !== commit) {
    throw new Error('Checkout must match the release tag.');
  }
  const ancestry = spawnSync('git', ['merge-base', '--is-ancestor', commit, 'refs/remotes/origin/master']);
  if (ancestry.status !== 0) {
    throw new Error('Release tag must belong to the history of origin/master.');
  }

  const packageRoot = process.env.PKG_ROOT ?? 'Assets/Packages/DraasGames.Core';
  const manifest = JSON.parse(readFileSync(path.join(packageRoot, 'package.json'), 'utf8'));
  if (manifest.version !== tag) {
    throw new Error(`Tag ${tag} does not match package version ${manifest.version}.`);
  }

  const lines = readFileSync(path.join(packageRoot, 'CHANGELOG.md'), 'utf8').split(/\r?\n/);
  const heading = `## [${tag}]`;
  const start = lines.findIndex(line => line === heading || line.startsWith(`${heading} - `));
  if (start === -1) {
    throw new Error(`CHANGELOG.md has no section for ${tag}.`);
  }
  let end = start + 1;
  while (end < lines.length && !lines[end].startsWith('## ')) end++;
  const notes = lines.slice(start + 1, end).join('\n').trim();
  if (!notes) {
    throw new Error(`CHANGELOG.md section for ${tag} is empty.`);
  }
  if (!process.env.RELEASE_NOTES_PATH || !process.env.GITHUB_OUTPUT) {
    throw new Error('RELEASE_NOTES_PATH and GITHUB_OUTPUT must be set.');
  }

  writeFileSync(process.env.RELEASE_NOTES_PATH, `# ${tag}\n\n${notes}\n`, 'utf8');
  appendFileSync(process.env.GITHUB_OUTPUT, `commit=${commit}\n`, 'utf8');
  console.log(`Validated release ${tag} at ${commit}.`);
} catch (error) {
  console.error(error.message);
  process.exitCode = 1;
}
