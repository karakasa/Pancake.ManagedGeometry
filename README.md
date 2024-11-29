# Pancake.ManagedGeometry
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fkarakasa%2FPancake.ManagedGeometry.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2Fkarakasa%2FPancake.ManagedGeometry?ref=badge_shield)


Pancake.ManagedGeometry is a geometry library to provide basic geometric objects through fully-managed code.

It has been the foundation of several in-house and released design plugins, features incl. daylight calcuation, visual analysis, etc. 

This is a personal project so API interfaces may change without notice.

## Objective

The core module should be ...

* Fully managed.
* Having neutral data model, and free of host applications.
* Free of platform-specific functions.
* Free of any proprietary dependency.
* Free of "viral" licenses, such as GPL.
* Fast.
* Reduce memory allocations as many as possible.

Objectives don't include:

* High abstractions of elements, including NURBS curve, BRep solid, AEC elements (walls, windows, etc.)
* Serialization support, which is designed to be the responsibility of caller. This repo may include tool libraries to help you with this.

## Similar projects you may be interested in
* [Hypar.Elements](https://github.com/hypar-io/Elements). A comprehensive library regarding BIM practice (and related geometric operations)
* [G-Shark](https://github.com/GSharker/G-Shark). A library about managed AEC geometric practices.


## License
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fkarakasa%2FPancake.ManagedGeometry.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2Fkarakasa%2FPancake.ManagedGeometry?ref=badge_large)