#!/usr/bin/env bash
# Pushes master and tags a release, which triggers the NuGet publish workflow.
#
# Usage: ./release.sh <version> [--retag]
#   ./release.sh 1.0.1
#   ./release.sh 1.1.0-beta.1
#   ./release.sh 1.0.0 --retag   # move an existing tag to HEAD (only if that version never reached nuget.org)
set -euo pipefail

version="${1:-}"
retag="${2:-}"

if [[ -z "$version" ]]; then
	echo "usage: $0 <version> [--retag]" >&2
	exit 1
fi

version="${version#v}"
tag="v$version"

if [[ -n "$(git status --porcelain)" ]]; then
	echo "error: working tree has uncommitted changes; commit them first" >&2
	exit 1
fi

if [[ "$(git branch --show-current)" != "master" ]]; then
	echo "error: not on master" >&2
	exit 1
fi

if git rev-parse -q --verify "refs/tags/$tag" >/dev/null; then
	if [[ "$retag" != "--retag" ]]; then
		echo "error: tag $tag already exists; bump the version, or pass --retag if it was never published" >&2
		exit 1
	fi
	git tag -d "$tag"
	git push origin ":refs/tags/$tag" || true
fi

git push origin master
git tag "$tag"
git push origin "$tag"

echo "Pushed $tag. Watch the run under the repo's Actions tab."
