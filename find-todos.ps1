$f = "todo.txt"

# look recursively for every .cs file
# and grep for the string "TODO:", show surrounding lines
# finally, output to the todo.txt file
gci *.cs -r | select-string "TODO:" -context 2, 7 > $f

try
{
    # try vim first
    vim $f
}
catch
{
    # will launch default editor
    explorer $f
}
