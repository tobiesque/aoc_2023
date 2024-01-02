import matplotlib.pyplot as plt
import networkx as nx
from networkx_viewer import Viewer
import networkx.algorithms.community as community
import numpy as np
from itertools import combinations
from nodes import data
import matplotlib.colors as mcolors

# Create a graph object
G = nx.Graph()

# Add nodes and edges to the graph
for node, connections in data.items():
    for connection in connections:
        G.add_edge(node, connection)

print(len(G))
