# Dungeon generation and Pathfinding demo

A project I worked on during my Algorithms course, which showcases various algorithms for dungeon generation and path finding.

<p align="center">
  <img src="Media/demo.gif"><br/>
  *<i>Low frame rate caused by gif limitations</i>*
</p>

## Overview

Throughout the course, we covered many fundamental algorithms and techiniques for solving common programming problems - from generating a dungeon with Binary Space Partitioning (BSP) to finding shorthest paths with the A* pathfinding algorithm. All done utilizing the [GXP 2D Game Engine](https://github.com/zntKI/AlgorithmsAssignment/tree/main/gxpengine) - a code-based small engine designed and developed by the teachers to introduce students to how a game engine may opperate.

## Features

### <br>Room splitting<br><br>

<p align="center">
  <img width="70%" src="Media/room_splitting.jpg"><br/>
</p>

### <br>Room splitting(Additional)<br><br>

<p align="center">
  *<i>Removes the biggest and smallest room/s and colors them by the number of connections(doors) they have</i>*<br>
  <img width="70%" src="Media/room_splitting_good.jpg"><br/>
</p>

### <br>Node generation<br><br>

<p align="center">
  <img width="70%" src="Media/node_generation.jpg"><br/>
</p>

### <br>Node Generation with Tiled View<br><br>

<p align="center">
  *<i>Generates a node for every walkable tile and links it to its neigbouring</i>*<br>
  <img width="70%" src="Media/node_generation_good.jpg"><br/>
</p>

### <br>Random Player Movement<br><br>

<p align="center">
  *<i>Player picks nodes randomly until it reaches its destination</i>*<br>
  <img width="70%" src="Media/agent_random.jpg"><br/>
</p>

### <br>Recursive Shortest(Node count) Pathfinding<br><br>

<p align="center">
  *<i>Recursively searches through <b>all<b/> nodes to make sure that the shortest path was found</i>*<br>
  <img width="70%" src="Media/agent_pathfinder_recursive.jpg"><br/>
</p>

### <br>BFS Shortest(Node count) Pathfinding<br><br>

<p align="center">
  *<i>Equally expands from the origin in the form of a square, searching for the destination</i>*<br>
  <img width="70%" src="Media/agent_pathfinder_bfs.jpg"><br/>
</p>

### <br>Dijkstra Shortest(Distance) Pathfinding<br><br>

<p align="center">
  *<i>Equally expands from the origin in the form of a circle, searching for the destination</i>*<br>
  <img width="70%" src="Media/agent_pathfinder_dijkstra.jpg"><br/>
</p>

### <br>A* Shortest(Distance/Node count) Pathfinding<br><br>

<p align="center">
  *<i>Combines both Dijkstra and Greedy search strategies to form a near perfect balance between accuracy and efficiency</i>*<br>
  <img width="70%" src="Media/agent_pathfinder_astart.jpg"><br/>
</p>
