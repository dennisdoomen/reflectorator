# Reflectify - Reflection extensions without causing dependency pains

[![](https://img.shields.io/github/actions/workflow/status/dennisdoomen/reflectify/build.yml?branch=main)](https://github.com/dennisdoomen/reflectify/actions?query=branch%main)
[![](https://img.shields.io/github/release/DennisDoomen/Reflectify.svg?label=latest%20release&color=007edf)](https://github.com/dennisdoomen/reflectify/releases/latest)
[![](https://img.shields.io/nuget/dt/Reflectify.svg?label=downloads&color=007edf&logo=nuget)](https://www.nuget.org/packages/Reflectify)
[![](https://img.shields.io/librariesio/dependents/nuget/Reflectify.svg?label=dependent%20libraries)](https://libraries.io/nuget/Reflectify)
[![GitHub Repo stars](https://img.shields.io/github/stars/dennisdoomen/reflectify)](https://github.com/dennisdoomen/reflectify/stargazers)
[![GitHub contributors](https://img.shields.io/github/contributors/dennisdoomen/reflectify)](https://github.com/dennisdoomen/reflectify/graphs/contributors)
[![GitHub last commit](https://img.shields.io/github/last-commit/dennisdoomen/reflectify)](https://github.com/dennisdoomen/reflectify)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/m/dennisdoomen/reflectify)](https://github.com/dennisdoomen/reflectify/graphs/commit-activity)
[![open issues](https://img.shields.io/github/issues/dennisdoomen/reflectify)](https://github.com/dennisdoomen/reflectify/issues)
![](https://img.shields.io/badge/release%20strategy-githubflow-orange.svg)

> [!TIP]
> If you like this package, consider sponsoring me through [Github Sponsors](https://github.com/sponsors/dennisdoomen)

## What's this about?

Reflectify offers a bunch of extension methods to provide information such as the properties or fields a type exposes and metadata about those members. It supports all major .NET versions and even understands explicltly implemented properties or properties coming from default interface implementations, a C# 8 feature.

## What's so special about that?

Nothing really, but it offers that functionality through a content-only NuGet package. In other words, you can use this package in your own packages, without the need to tie yourself to the Reflectify package. 