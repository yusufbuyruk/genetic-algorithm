# Genetic Algorithm
Genetic Algorithm for Warehouse Location Problem

I developed a fast and powerful genetic algorithm to solve warehouse location problem.

---
# The Warehouse Location Problem

## Introduction

The Warehouse Location Problem (WLP) is a classic optimization problem. In this problem, a distribution company uses warehouses to provide goods to many different customers. The goal is to determine which warehouses will be the most cost-effective for serving the customers. Each warehouse has different costs and storage capabilities, which adds complexity to the problem.

## Assignment

You are tasked with designing an algorithm to solve the WLP. The problem can be mathematically formulated as follows:

- There are `N = 0 ... n−1` warehouses to choose from.
- There are `M = 0 ... m−1` customers that need to be served.
- Each warehouse `w ∈ N` has a capacity `cap_w` and a setup cost `s_w`.
- Each customer `c ∈ M` has a demand `d_c` and a travel cost `t_cw`, which depends on which warehouse serves them.

Lastly, all customers must be served by exactly one warehouse. Let `a_w` be a set variable denoting the customers assigned to warehouse `w`. The problem is to minimize the costs.

## Problem Formulation

The objective is to minimize the total cost, which consists of both the setup cost and the transportation cost from warehouses to customers. The problem can be formally expressed as:


