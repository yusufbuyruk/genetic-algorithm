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

![Warehouse Location](readme-images/warehouse-location.jpg.png)

This represents the Warehouse Location Problem (WLP), where:

- **`N`** is the set of warehouses.
- **`M`** is the set of customers.
- **`a_w`** is the set of customers assigned to warehouse **`w`**.
- **`s_w`** is the setup cost for warehouse **`w`**.
- **`t_cw`** is the transportation cost of customer **`c`** from warehouse **`w`**.
- **`d_c`** is the demand of customer **`c`**.
- **`cap_w`** is the capacity of warehouse **`w`**.

The goal is to minimize the total cost, subject to the constraints of warehouse capacities and customer assignments.

### Data Format

The input consists of `|N| + 2|M| + 1` lines. The first line contains two numbers, `|N|` followed by `|M|`. 

The first line is followed by `|N|` lines, where each line represents a warehouse capacity `cap_w` and setup cost `s_w`.

The last `2|M|` lines capture the customer information. Each customer block begins with a line containing one number, the customer’s demand `d_c`. The following line has `|N|` values, one for each warehouse. These values capture the cost to service that customer from each warehouse, `t_cw`.

