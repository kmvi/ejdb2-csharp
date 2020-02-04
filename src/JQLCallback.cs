using System;
using System.Collections.Generic;
using System.Text;

namespace Ejdb2
{
    /// <summary>
    /// Called on every JSON record in result set.
    /// </summary>
    /// <remarks>
    /// Implementor can control iteration behaviour by returning a step getting next record:
    /// <list type="bullet">
    /// <item>
    /// <term><c>1</c></term>
    /// <description>Go to the next record</description>
    /// <term><c>N</c></term>
    /// <description>move forward by <c>N</c> records</description>
    /// <term><c>-1</c></term>
    /// <description>Iterate current record again</description>
    /// <term><c>-2</c></term>
    /// <description>Go to the previous record</description>
    /// <term><c>0</c></term>
    /// <description>Stop iteration</description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="id">Current document identifier</param>
    /// <param name="json">Document JSON body as string</param>
    /// <returns>Number of records to move</returns>
    public delegate uint JQLCallback(long id, string json);
}
