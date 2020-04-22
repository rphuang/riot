#!/usr/bin/python3
# File name   : piIotNode.py
# Description : base class for all Raspberry Pi IOT nodes 

from iotServerLib import iotNode

class PiIotNode(iotNode.IotNode):
    """ the base class for all Raspberry Pi IOT nodes """
    def __init__(self, name, parent):
        """ construct a PiIotNode
        name - the name of the node
        parent - parent IotNode object. None for root node.
        """
        super(PiIotNode, self).__init__(name, parent)




