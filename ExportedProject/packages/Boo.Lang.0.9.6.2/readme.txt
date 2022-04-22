The boo programming language (c) 2009-2014 Rodrigo Barreto de Oliveira (rbo@acm.org)

How to Start
============

Folder bin/ contains the latest version that passed all
the tests and could be successfully used to rebuild the
system.

For a brief description of the project and its goals
take a look at docs/BooManifesto.sxw and the Boo Primer
at docs/BooPrimer.pdf.

Folder tools/ndoc/ contains an updated version of the ndoc
documentation system that creates documentation in
various formats from assemblies and corresponding
XML documentation.

Running and compiling code
==========================

To execute a boo script run:

	booi <script> [args]
	
For instance:

	booi examples/hw.boo	
	
You can also have booi to read from stdin by typing:

	booi -
	
Additionally, boo also comes with an interactive shell that
provides you with the opportunity to use the interpreter
interactively.

	booish
	
After starting the shell you may type in command "h" to
display some hopefully helpful information on the
particular abilities of this shell. The startup environment
of the shell may be configured editing file booish.rsp that
also resides in the bin/ folder (must be side-by-side to
the program).

You can generate .net assemblies by using `booc` (either
the `booc.exe` utility or the `booc nant` task):

	booc -output:build/hello.exe examples/hw.boo	
	
If you want to simply see the transformations applied to
your code by the compiler use the boo pipeline, run:

	booc -p:boo examples/replace.boo	
	
Folder bin/ also contains support modules for the NANT and
MSBUILD build systems. You will have to define environment
variable "BooBinPath" in such a way that

	$(BooBinPath)\Boo.Microsoft.Build.targets
	
points at the definitions of the MSBuild targets referring to
Boo.

This distribution has been signed with an unpublished key
(that should be accessable only by the distributor).

More Information
================

http://boo.codehaus.org/

Contributors
============

See: https://github.com/hmah/boo/graphs/contributors

Differences to the original distribution
=======================================

- Fix of issue BOO-1078 Cannot use enumerations as attribute parameters.
- Version information on assemblies is now pasted into the native resources. Thus, Windows Explorer can read and display this.
- booish: New help display
- booish: describe() now looks for XML documentation of referenced assemblies.
- booish: You can now start autocompletion from the start of the line on (then the list of suggestion will offer globals and namespaces).
- booish: Autocompletion offers suggestions in a scrollable list with one offer per row (better overview).
- booish: Solved some problems with managing input into the shell. Added mode to allow users to paste content into the shell.
- booish: booish.rsp now offers additional options to configure the startup behaviour of the shell.
- booish: Added means to add commands to the shell. Added booish.mod.os to demonstrate this. This module provides commands like "cd" and "dir".
- boo: range(-1) now is an empty enumeration (instead of raising an exception).
- boo: Operator "isa" now also works with structs. This is important to avoid exceptions on casts.
- boo: Ctors and dtors now do not require a "def" (but you still can provide this). This keyword is redundance (not very wrist friendly).
- boo: Macro "property" now also work for static classes.
- boo: Added macro "getproperty" that keeps the setter private.
- boo: array(int, null) now return null (without an exception). This spares some if-then on using array for casting.
- boo: Resolved issues #57 and #58.

