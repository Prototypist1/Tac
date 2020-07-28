# Tac

Tac is a proof of concept programming language that tests a flat approach to sharing logic between objects.

![Tac reuse model](https://raw.githubusercontent.com/Prototypist1/Tac/master/tac-reuse-model.png)

Tac has no inheritance or class hierarchies; instead objects that share functionality are constructed using the same "building block" method-blueprints.

This approach pushes most of an app's logic from classes into method-blueprints. Making it easier to access, reuse and test.

# The Specifics

Tac has four core structures.

- Objects - group/organize data
- Types - describe and classifying objects
- Methods - group/organize logic
- Method-blueprints - Create methods that share logic but act with respect to different contexts. They allow us to easily reuse instance methods across different kinds of object. A capacity traditionally provided by inheritance.

It's all pretty familiar stuff. Objects, types and method are more or less what you expect and method-blueprints are just glorified method factories.

## Objects

Objects help us organize information. They do this by grouping related values so that we can think of as a single entity.

Objects are implemented as a set of properties. Where a property is a named, typed, value that represents an aspect of its object. If an object models a person, it's properties might include their name and age.

To create an object, we write:
```
object {
    string first-name := "Jared",
    string last-name := "Dunn",
    method<named, string> say-hello := ...
}
```
Let's break this constructor down. We start with "Object" because that is what we want to make. We follow with brackets containing a comma separated list of properties. The first-name of our object is "Jared", the last-name is "Dunn", and it has a method called SayHello (more on methods later).

If this constructor looks strange to you, it is likely because Tac has no concept of class. When you create an object in Tac you a simultaneously defining its structure and populating it.

## Types

Types help us describe objects. They do this by classifying them as "of the type" or "not of the type". This membership is determined by the objects structure (an approach aptly named structural typing).

Concretely, a Type is a collection of name-type pairs. Each pair represents the name and type of a required property. Objects with property that match all the pairs are considered "of the type".

To define a type:
```
type named {
    stringfirst-name,
    stringlast-name
}
```
As with objects, we start with what we want to make � in this case, a type. Next comes what we want to call our type. We finish with what an object must have to be considered "Named": a string called first-name and a string called last-name.

In addition to user defined types like the one above, Tac comes with several _primitive types_ like int, string, Method and Method-blueprint.

## Methods

Methods are a reusable series of operations that that act on an input to produce an output.

To create a method, we write:
```
method<named, string> input {
    input.first-name + " " +input.last-name return;
}
```
As always, we start with what we are trying to make, only this time that _what_ is a little more complex. Methods that require different input types or produce different output types are not interchangeable. Thus, when referring to methods, we always include the input and output types next to "Method" in a pair of angle brackets. We refer to these as type parameters. The first type parameter is the method's input type (Named) the second is the method's return type (String). We finish with the input parameter (input) and some code that concatenates the input's first-name and last-name.

Before we talk about Method-blueprints, it is important to understand that Methods always act with respect to the context in which they were created. This is best demonstrated. Say a method is defined alongside some variable i like so:
```
i := 0;
add-to-i := method<int, int> to-add {
    i := i + to-add;
    i return;
}
```
This method adds some value to `i`. Crucially, it always modifies the `i` that was defined next to it, even if a different `i` is defined where the method is being called. Moreover, the value of `i` persists from one call to the next so calling this method repeatedly will return different values.

The method modifies that specific `i` because it is bound to the context in which it was created. This is generally very useful but, it can be troublesome. For example, if we wanted to create two copies of the method above bound to different `i`s, it would require duplicate code or worse, getting fancy. This problem becomes very real when you need to reuse logic in different objects.

## Method-blueprints

Method-blueprints make it easy to reuse the same logic in multiple places without duplicate code. A blueprint is a factory that creates methods with the same logic but bound to different contexts.

Method-blueprints are really just method. They take a context as their input and return a method that acts with respect to that context.

Unsurprisingly, defining a method-blueprint is very similar to defining a method:
```
say-hello-blueprint := blueprint<named, named, string> context subject {
    "Hello " + subject.frist-name + ", it's " +context.first-name return;
}
```
Unlike methods, method-blueprints have three type parameters: the first is contexts type, the second is the input of the returned method, and the third is the output of the return method. An blueprint definition also includes the name of the context, the name of the created functions input, and the code that will be the body of the method.

To create a method, call the blueprint like you would a method and pass in an object you want it to act with regard to.

```
object {
    string first-name := "Jared",
    string last-name := "Dunn",
    method<named, string> say-hello := this > say-hello-blueprint
}
```
---

In summary: objects organize your data, types describe objects, methods act, and method-blueprints make it easy to reuse methods in different objects.