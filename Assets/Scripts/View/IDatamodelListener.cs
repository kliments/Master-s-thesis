using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDatamodelListener {

    void datamodelFilterChange(NumericDatamodel datamodel);     // Filters of the data model have changed.
    void datamodelMinMaxChange(NumericDatamodel datamodel);     // Min or max values of some axes have changed. May be useful for labeling of axises.
}
