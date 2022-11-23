using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathEdit.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CommandFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandLine"></param>
        /// <returns></returns>
        static public ICommand Create(string commandLine)
        {
            // TODO: Create Command (Needs reflection)
            // add
            // edit
            // delete
            // clear (!?)
            // move
            // swap
            // save
            // revert
            // exit (save + quit)
            // quit
            // undo ?

            ICommand command = null;

            try
            {
                if (!string.IsNullOrEmpty(commandLine))
                {
                    // TODO: Strip out command Name
                    string[] bits = commandLine.Split(" ".ToCharArray());

                    if (bits.Length > 0)
                    {
                        string name = bits.Length > 0 ? bits[0] : null;

                        if (!string.IsNullOrEmpty(name))
                        {
                            CommandDefinition[] commandDefs = BaseCommand.GetCommands();

                            foreach (var def in commandDefs)
                            {
                                if (def.IsCommandName(name))
                                {
                                    command = Activator.CreateInstance(def.CommandType) as ICommand;
                                    break;
                                }
                            }
                        }
                    }

                    // Configure
                    if (command != null)
                    {
                        for (int i = 1; i < bits.Length; ++i)
                            command.AddParameter(bits[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                command = null;
            }

            return command;
        }
    }
}
