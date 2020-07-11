## Introduction
Feature.Manager is exactly what it sounds like, basically you wrap your features inside feature manager, and based on certain criteria, you can enable/disable features.
You can also do A/B testing with this, but it's not included on phase 1 of this project, once feature manager is stable where one can rely on this to be production ready,
then another scope will be to check the actual business value.


Tests, builds and deployment will be done on linux machine, but windows should work too,
However a project as small as this shouldn't be a problem to support on all OSes.
CI will be done only on linux on a docker container.

## Why use this?
The reason I started this project is so that I can use this myself, I don't want new features to creep with bugs and before I know it it's exposed to public.
I want to make sure the feature is working okay by turning the feature to only 1% of the customer, then slowly increasing later as I gain confidence.

## What clients will be available
Initially this will most likely only produce C# client,
But we'll be documenting how it works, and how to calculate the data, and a flowchart will be made available, so you can start your own client for any language, this will expose endpoints based on swagger as well.

## Timeline
Well, this isn't a full time project which I can work on, so the more contribution I get the faster project will flow.

I'm thrilled about this project and want to use for myself anyway, there'll be continuous development.
Further contribution will only make this grow better and faster.
I want to see this project be production ready, where you clone it,
and start writing production ready code without setting up. Once I reach a phase I'm happy with for feature management, I'll only work on making this more business oriented if I get the need for it.


after a while no more development will be needed on this project, but rather only frontend can be made better.

## Used packages

## Getting Started
Simply clone this repository, and open in Rider or Visual Studio Code and start building.

## Contribution
Please follow the existing coding standards which is being followed, no trailing whitespaces, edge cases goes to if conditions,
follow line of sight rule. Happy path is always straight down, only short circuit (early exits) the error path unless there's a strong reason not to.

## More Documentation
Before a feature is worked on, I'll try to document on what needs to be done, after the feature is ready,
I'll try to finalize the documentation. If there's something missing, please contribute.


## ER Diagram:
```mermaidjs
erDiagram
        USER }|--|{ EXPERIMENT : has
        EXPERIMENT ||--|| EXPERIMENT-ID : identified-by
		EXPERIMENT ||..|| HYPOTHESIS : has
		EXPERIMENT ||..|| DESCRIPTION : has
		EXPERIMENT ||--|| EXPERIMENT-TOKEN : has
		EXPERIMENT ||--o{ EXPERIMENT-RUN : has
		EXPERIMENT-RUN || -- || RUN-ID : has
		EXPERIMENT-RUN || .. || ALLOCATION : has
		EXPERIMENT-RUN || .. || START-AT : starts
		EXPERIMENT-RUN || .. || END-AT : ends
		EXPERIMENT-RUN || .. || RUN-TOKEN : has
```
![image](https://mermaid.ink/svg/eyJjb2RlIjoiZXJEaWFncmFtXG4gICAgICAgIFVTRVIgfXwtLXx7IEVYUEVSSU1FTlQgOiBoYXNcbiAgICAgICAgRVhQRVJJTUVOVCB8fC0tfHwgRVhQRVJJTUVOVC1JRCA6IGlkZW50aWZpZWQtYnlcblx0XHRFWFBFUklNRU5UIHx8Li58fCBIWVBPVEhFU0lTIDogaGFzXG5cdFx0RVhQRVJJTUVOVCB8fC4ufHwgREVTQ1JJUFRJT04gOiBoYXNcblx0XHRFWFBFUklNRU5UIHx8LS18fCBFWFBFUklNRU5ULVRPS0VOIDogaGFzXG5cdFx0RVhQRVJJTUVOVCB8fC0tb3sgRVhQRVJJTUVOVC1SVU4gOiBoYXNcblx0XHRFWFBFUklNRU5ULVJVTiB8fCAtLSB8fCBSVU4tSUQgOiBoYXNcblx0XHRFWFBFUklNRU5ULVJVTiB8fCAuLiB8fCBBTExPQ0FUSU9OIDogaGFzXG5cdFx0RVhQRVJJTUVOVC1SVU4gfHwgLi4gfHwgU1RBUlQtQVQgOiBzdGFydHNcblx0XHRFWFBFUklNRU5ULVJVTiB8fCAuLiB8fCBFTkQtQVQgOiBlbmRzXG5cdFx0RVhQRVJJTUVOVC1SVU4gfHwgLi4gfHwgUlVOLVRPS0VOIDogaGFzXG4iLCJtZXJtYWlkIjp7InRoZW1lIjoiZGVmYXVsdCJ9LCJ1cGRhdGVFZGl0b3IiOmZhbHNlfQ)
