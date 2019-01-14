﻿const SRD = window["storm-react-diagrams"];

class BaseDispatchModel extends SRD.PortModel {
    model = null;
    linked = false;
    linkColor = null;

    constructor(model, linkColor) {
        if (model) {
            super(model.name, "dispatch");
            this.model = model;
        } else {
            super("", "dispatch");
        }

        this.linkColor = linkColor;
    }

    deSerialize(other, engine) {
        super.deSerialize(other, engine);
        this.model = other.model;
        this.linked = other.linked;
        this.linkColor = other.linkColor;
    }

    serialize() {
        return _.merge(super.serialize(), {
            model: this.model,
            linked: this.linked,
            linkColor: this.linkColor
        });
    }

    canLinkToPort(other) {
        return other instanceof BaseInvokeModel;
    }

    createLinkModel() {
        return new BaseCallerLinkModel(this.linkColor);
    }

    addLink(link) {
        super.addLink(link);
        this.linked = true;
    }

    removeLink(link) {
        super.removeLink(link);
        if (_.size(this.links) === 0) {
            this.linked = false;
        }
    }
}