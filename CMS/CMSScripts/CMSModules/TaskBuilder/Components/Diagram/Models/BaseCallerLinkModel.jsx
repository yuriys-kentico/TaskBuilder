﻿const SRD = window["storm-react-diagrams"];

class BaseCallerLinkModel extends SRD.LinkModel {
    width = 4;
    curvyness = 50;
    color = null;

    constructor(linkColor) {
        super("caller");
        this.color = linkColor;
    }

    serialize() {
        return _.merge(super.serialize(), {
            width: this.width,
            curvyness: this.curvyness,
            color: this.color
        });
    }

    deSerialize(ob, engine) {
        super.deSerialize(ob, engine);
        this.width = ob.width;
        this.curvyness = ob.curvyness;
        this.color = ob.color;
    }

    setTargetPort(port) {
        if (port && this.sourcePort) {
            if (this.sourcePort.type === "dispatch" && port.type === "invoke") {
                this.type = "caller";
            }
        }

        super.setTargetPort(port);
    }

    addLabel(label) {
        if (label instanceof SRD.LabelModel) {
            return super.addLabel(label);
        }

        let labelOb = new SRD.DefaultLabelModel();
        labelOb.setLabel(label);

        return super.addLabel(labelOb);
    }

    setWidth(width) {
        this.width = width;

        this.iterateListeners((listener, event) => {
            if (listener.widthChanged) {
                listener.widthChanged({ ...event, width: width });
            }
        });
    }

    setColor(colors) {
        this.color = color;

        this.iterateListeners((listener, event) => {
            if (listener.colorChanged) {
                listener.colorChanged({ ...event, color: color });
            }
        });
    }
}